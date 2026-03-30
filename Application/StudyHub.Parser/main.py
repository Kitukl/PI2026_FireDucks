from fastapi import FastAPI, HTTPException
from playwright.async_api import async_playwright
import camelot
import tempfile
import asyncio
import json
import os
import re
from abc import ABC, abstractmethod


app = FastAPI()

class FacultyHelper(ABC):
    @property
    @abstractmethod
    def base_url(self) -> str:
        pass

    @abstractmethod
    def is_match(self, group_name: str) -> bool:
        pass

    @abstractmethod
    def get_search_criteria(self, group_name: str) -> tuple[str, str]:
        pass

class AmiFacultyHelper(FacultyHelper):
    @property
    def base_url(self) -> str:
        return "https://ami.lnu.edu.ua/students/rozklad-zanyat"

    def __init__(self):
        self._specialization_map = {
            "ПМІ": "КН", "ПМО": "СО", "ПМП": "ПМ", "ПМК": "КБ", "ПМА": "СА"
        }

    def is_match(self, group_name: str) -> bool:
        return group_name.upper().startswith("ПМ")

    def get_search_criteria(self, group_name: str) -> tuple[str, str]:
        match = re.match(r"^([А-ЯІЇЄ]+)-?(\d+)", group_name.upper())
        if not match:
            raise ValueError(f"Uncorrect group name format for AMI: {group_name}")

        prefix, number = match.groups()
        course_num = number[0]
        site_id = self._specialization_map.get(prefix, "")
        return course_num, site_id


FACULTIES = [
    AmiFacultyHelper()
]


def get_faculty_for_group(group_name: str) -> FacultyHelper:
    for faculty in FACULTIES:
        if faculty.is_match(group_name):
            return faculty
    raise ValueError(f"Could not find faculty for group name: {group_name}")


async def download_pdf(faculty: FacultyHelper, group_name: str) -> str:
    course_num, spec_id = faculty.get_search_criteria(group_name)

    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(accept_downloads=True)
        page = await context.new_page()

        await page.goto(faculty.base_url)

        filter_course = f"{course_num} курс"
        locator = page.locator("a[href$='.pdf']").filter(has_text=filter_course)

        if spec_id:
            locator = locator.filter(has_text=spec_id)

        if await locator.count() == 0:
            await browser.close()
            raise FileNotFoundError(f"Could not find PDF schedule for course and spec: ({filter_course}, {spec_id})")

        async with page.expect_download() as download_info:
            await locator.first.click()

        download = await download_info.value
        temp_file = tempfile.NamedTemporaryFile(delete=False, suffix=".pdf")
        temp_path = temp_file.name
        temp_file.close()

        await download.save_as(temp_path)
        await browser.close()

    return temp_path


def parse_schedule_sync(pdf_path: str, target_group: str) -> list:
    try:
        tables = camelot.read_pdf(
            pdf_path,
            pages="all",
            flavor="lattice",
            copy_text=['h', 'v'],
            line_scale=90,
            strip_text="\n"
        )

        if tables.n == 0:
            raise Exception("Не знайдено жодної таблиці в PDF")

        df = tables[0].df
        df.columns = df.iloc[0]
        df = df[1:].reset_index(drop=True)

        all_columns = list(df.columns)
        if len(all_columns) <= 2:
            raise Exception("Не вистачає колонок для груп (очікується >2)")

        group_columns = all_columns[2:]
        day_col = all_columns[0]
        time_col = all_columns[1]

        DAY_WORDS = {
            "Понеділок": ["По", "Пон"],
            "Вівторок": ["Вівт"],
            "Середа": ["С", "Сер"],
            "Четвер": ["Чет", "Чт"],
            "П'ятниця": ["П’", "Пят", "П'"],
        }

        def get_day(text):
            for day, aliases in DAY_WORDS.items():
                if any(alias in text for alias in aliases):
                    return day
            return None

        def parse_subject_cell(text: str):
            text = re.sub(r"\s{2,}", " ", text.replace("\n", " ")).strip(" ,.;")
            text = re.sub(r"([а-яa-z]+)\.,?(\d)", r"\1, \2", text, flags=re.I)

            match = re.search(
                r"""^(?P<name>[А-ЯA-ZЇІЄҐа-яa-z0-9().,\-’'– ]+?)
                     (?:\s+(?P<type>лекція|лаб\.?|практ\.?|семінар|лек\.?|ла|лек|прак|практика))?
                     [,. ]*
                     (?P<room>\d{2,3}[абв]?)?
                     [,. ]*
                     (?P<teacher>(?:доц\.|проф\.|ас\.|ст\. викл\.|викл\.).*)?
                     $""",
                text,
                flags=re.I | re.X,
            )

            if not match:
                return {"subject": text, "type": "", "room": "", "teachers": []}

            name = (match.group("name") or "").strip(" ,.-")
            type_ = (match.group("type") or "").strip(" ,.-")
            room = (match.group("room") or "").strip(" ,.-")
            teacher_raw = (match.group("teacher") or "").strip(" ,.-")

            teachers = []
            if teacher_raw:
                teachers = [
                    t.strip()
                    for t in re.split(
                        r"(?:,\s*|\s{2,}|\s+(?=доц\.|проф\.|ас\.|ст\. викл\.|викл\.))",
                        teacher_raw
                    )
                    if t.strip()
                ]

            return {
                "subject": name,
                "type": type_,
                "room": room,
                "teachers": teachers
            }

        def normalize_time(raw_time: str) -> str:
            raw_time = re.sub(r"^[IVX]+\s*", "", raw_time).strip()
            times = re.findall(r"\d{3,4}", raw_time)

            def format_t(t):
                if len(t) == 3:
                    return f"{t[0]}:{t[1:]}"
                elif len(t) == 4:
                    return f"{t[:2]}:{t[2:]}"
                return t

            if len(times) >= 2:
                return f"{format_t(times[0])} - {format_t(times[1])}"
            elif len(times) == 1:
                return format_t(times[0])
            else:
                return raw_time

        target_group_clean = re.sub(r"\s*[-–—]\s*", "-", target_group.strip().upper())
        target_col = None
        for col in group_columns:
            col_clean = re.sub(r"\s*[-–—]\s*", "-", str(col).strip().upper())
            if col_clean == target_group_clean:
                target_col = col
                break

        if not target_col:
            raise KeyError(f"Group {target_group} cannot be found in PDF")

        flat_lessons = []

        day_to_number = {"Понеділок": 1, "Вівторок": 2, "Середа": 3, "Четвер": 4, "П'ятниця": 5, "Субота": 6,
                         "Неділя": 7}

        for _, row in df.iterrows():
            day_str = get_day(str(row[day_col]).strip())
            time_str = normalize_time(str(row[time_col]).strip())

            if not day_str or day_str == "nan": continue

            cell = str(row[target_col]).strip()
            if not cell or cell.lower() == "nan": continue

            times = time_str.split("-")
            start_time = times[0].strip() if len(times) > 0 else "00:00"
            end_time = times[1].strip() if len(times) > 1 else "00:00"

            subjects = [s.strip() for s in re.split(r'\n+', cell) if s.strip()]
            for subj in subjects:
                subj = re.sub(r"\s{2,}", " ", subj).strip()
                parsed_cell = parse_subject_cell(subj)

                flat_lessons.append({
                    "day": day_to_number.get(day_str, 1),
                    "startTime": start_time,
                    "endTime": end_time,
                    "subjectName": parsed_cell["subject"],
                    "lessonType": parsed_cell["type"],
                    "room": parsed_cell["room"],
                    "teachers": [{"surname": t, "name": ""} for t in parsed_cell["teachers"]]
                })

        return flat_lessons
    finally:
        if os.path.exists(pdf_path): os.remove(pdf_path)


@app.get("/api/v1/schedule/parse")
async def parse_group_schedule(group: str):
    try:
        faculty = get_faculty_for_group(group)
        pdf_path = await download_pdf(faculty, group)
        lessons = await asyncio.to_thread(parse_schedule_sync, pdf_path, group)

        return {
            "groupName": group,
            "lessons": lessons
        }
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))