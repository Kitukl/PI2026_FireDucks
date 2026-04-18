// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
	const filterControls = document.querySelectorAll(".js-task-filter-control");

	function normalizeStatus(value) {
		return (value || "")
			.trim()
			.toLowerCase()
			.replace(/\s+/g, "-");
	}

	function getCardStatus(card) {
		const explicit = normalizeStatus(card.dataset.status);
		if (explicit) {
			return explicit;
		}

		const column = card.closest(".taskboard-column");
		const heading = normalizeStatus(column ? column.querySelector("h2")?.textContent : "");

		if (heading === "to-do") return "todo";
		if (heading === "in-progress") return "in-progress";
		if (heading === "for-review") return "for-review";
		if (heading === "done") return "done";

		return "todo";
	}

	function deriveOffsetByCode(taskCode, status) {
		const code = taskCode || "TASK";
		let seed = 0;
		for (let i = 0; i < code.length; i += 1) {
			seed += code.charCodeAt(i);
		}

		if (status === "done") {
			return seed % 2 === 0 ? 0 : -1;
		}

		const pattern = [0, 1, 2, 3, -1, 4];
		return pattern[seed % pattern.length];
	}

	function toIsoDate(date) {
		const y = date.getFullYear();
		const m = String(date.getMonth() + 1).padStart(2, "0");
		const d = String(date.getDate()).padStart(2, "0");
		return y + "-" + m + "-" + d;
	}

	function startOfDay(date) {
		return new Date(date.getFullYear(), date.getMonth(), date.getDate());
	}

	function ensureCardDeadline(card) {
		if (card.dataset.deadline) {
			return;
		}

		const status = getCardStatus(card);
		const code = (card.querySelector("footer")?.textContent || "").trim();
		const offset = deriveOffsetByCode(code, status);
		const target = new Date();
		target.setDate(target.getDate() + offset);
		card.dataset.deadline = toIsoDate(target);
	}

	function evaluateDeadline(deadlineIso, status, completedAtIso) {
		const today = startOfDay(new Date());
		const deadline = startOfDay(new Date(deadlineIso));
		const dayMs = 24 * 60 * 60 * 1000;
		const diff = Math.round((deadline - today) / dayMs);

		if (status === "done") {
			const completedAt = completedAtIso ? startOfDay(new Date(completedAtIso)) : today;
			if (completedAt <= deadline) {
				return { text: "✓", state: "is-done" };
			}
			return { text: "✕", state: "is-late" };
		}

		if (diff < 0) {
			return { text: "✕", state: "is-late" };
		}

		if (diff === 0) {
			return { text: "Today", state: "is-today" };
		}

		if (diff > 365) {
			const years = Math.floor(diff / 365);
			if (years > 5) {
				return { text: "N/A", state: "is-unavailable" };
			}

			return { text: String(years) + "Y", state: "is-countdown" };
		}

		return { text: String(diff) + "D", state: "is-countdown" };
	}

	function ensureIndicator(card) {
		let indicator = card.querySelector(".task-deadline-indicator");
		if (indicator) {
			return indicator;
		}

		const legacy = card.querySelector(".task-card-status, .task-card-state");
		if (legacy) {
			legacy.classList.remove("task-card-status", "task-card-state", "soft", "dark");
			legacy.classList.add("task-deadline-indicator");
			return legacy;
		}

		const header = card.querySelector("header");
		if (!header) {
			return null;
		}

		indicator = document.createElement("span");
		indicator.className = "task-deadline-indicator";
		header.appendChild(indicator);
		return indicator;
	}

	function renderCardDeadline(card) {
		ensureCardDeadline(card);
		const indicator = ensureIndicator(card);
		if (!indicator) {
			return;
		}

		const status = getCardStatus(card);
		const result = evaluateDeadline(card.dataset.deadline, status, card.dataset.completedAt);
		indicator.textContent = result.text;
		indicator.classList.remove("is-done", "is-late", "is-today", "is-countdown");
		indicator.classList.add(result.state);
	}

	function getSubject(card) {
		const subjectNode = card.querySelector("p");
		const rawValue = subjectNode?.dataset.subject || subjectNode?.textContent || "";
		return rawValue.trim().toLowerCase();
	}

	function getTaskTitle(card) {
		const titleNode = card.querySelector("h3");
		return (titleNode ? titleNode.textContent : "").trim().toLowerCase();
	}

	function getSearchQuery(board) {
		const search = board.querySelector(".taskboard-search input[type='search']");
		return (search ? search.value : "").trim().toLowerCase();
	}

	function getActiveScope(board) {
		const active = board.querySelector(".js-task-scope-filter.active");
		return active ? active.dataset.scope : "all";
	}

	function isGroupTaskCard(card) {
		return Boolean(card.querySelector(".task-group-label"));
	}

	function matchesScope(card, scope) {
		if (scope === "group") {
			return isGroupTaskCard(card);
		}

		if (scope === "own") {
			return !isGroupTaskCard(card);
		}

		return true;
	}

	function applyBoardFilters(board) {
		const checkboxes = board.querySelectorAll(".js-task-filter-checkbox");
		const enabledSubjects = new Set(
			Array.from(checkboxes)
				.filter(function (item) { return item.checked; })
				.map(function (item) { return item.value.toLowerCase(); })
		);
		const scope = getActiveScope(board);
		const searchQuery = getSearchQuery(board);

		const cards = board.querySelectorAll(".task-card");
		cards.forEach(function (card) {
			const subjectMatch = enabledSubjects.has(getSubject(card));
			const scopeMatch = matchesScope(card, scope);
			const titleMatch = searchQuery === "" || getTaskTitle(card).includes(searchQuery);
			card.classList.toggle("is-hidden", !(subjectMatch && scopeMatch && titleMatch));
		});
	}

	function updateToggleButton(allButton, checkboxes) {
		const allChecked = Array.from(checkboxes).every(function (item) { return item.checked; });
		allButton.textContent = allChecked ? "Unselect all" : "Select all";
	}

	filterControls.forEach(function (control) {
		const board = control.closest(".taskboard-page");
		const toggle = control.querySelector(".js-task-filter-toggle");
		const dropdown = control.querySelector(".taskboard-filter-dropdown");
		const allButton = control.querySelector(".js-task-filter-all");
		const checkboxes = control.querySelectorAll(".js-task-filter-checkbox");

		if (!board || !toggle || !dropdown || !allButton || checkboxes.length === 0) {
			return;
		}

		toggle.addEventListener("click", function () {
			const isOpen = !dropdown.hidden;
			dropdown.hidden = isOpen;
			toggle.setAttribute("aria-expanded", (!isOpen).toString());
		});

		checkboxes.forEach(function (checkbox) {
			checkbox.addEventListener("change", function () {
				applyBoardFilters(board);
				updateToggleButton(allButton, checkboxes);
			});
		});

		allButton.addEventListener("click", function () {
			const allChecked = Array.from(checkboxes).every(function (item) { return item.checked; });
			checkboxes.forEach(function (item) {
				item.checked = !allChecked;
			});

			applyBoardFilters(board);
			updateToggleButton(allButton, checkboxes);
		});

		document.addEventListener("click", function (event) {
			if (!control.contains(event.target)) {
				dropdown.hidden = true;
				toggle.setAttribute("aria-expanded", "false");
			}
		});

		applyBoardFilters(board);
		updateToggleButton(allButton, checkboxes);
	});

	const boards = document.querySelectorAll(".taskboard-page");
	boards.forEach(function (board) {
		const scopeButtons = board.querySelectorAll(".js-task-scope-filter");
		const searchInput = board.querySelector(".taskboard-search input[type='search']");

		if (searchInput) {
			searchInput.addEventListener("input", function () {
				applyBoardFilters(board);
			});
		}

		if (scopeButtons.length === 0) {
			applyBoardFilters(board);
			return;
		}

		scopeButtons.forEach(function (button) {
			button.addEventListener("click", function () {
				scopeButtons.forEach(function (item) {
					item.classList.remove("active");
					item.setAttribute("aria-pressed", "false");
				});

				button.classList.add("active");
				button.setAttribute("aria-pressed", "true");
				applyBoardFilters(board);
			});

			if (button.classList.contains("active")) {
				button.setAttribute("aria-pressed", "true");
			} else {
				button.setAttribute("aria-pressed", "false");
			}
		});

		applyBoardFilters(board);
	});

	const boardCards = document.querySelectorAll(".taskboard-page .task-card");
	boardCards.forEach(function (card) {
		renderCardDeadline(card);

		card.addEventListener("dragstart", function (event) {
			const board = card.closest(".taskboard-page");
			if (!board) {
				event.preventDefault();
				return;
			}

			card.classList.add("is-dragging");
			card.dataset.sourceStatus = getCardStatus(card);
			card.dataset.sourceColumnStatus = card.closest(".taskboard-column")?.dataset.columnStatus || card.dataset.sourceStatus;
			if (event.dataTransfer) {
				event.dataTransfer.effectAllowed = "move";
				event.dataTransfer.setData("text/plain", card.dataset.taskId || "");
			}
		});

		card.addEventListener("dragend", function () {
			card.classList.remove("is-dragging");
			card.dataset.justDragged = "true";
			setTimeout(function () {
				delete card.dataset.justDragged;
			}, 180);
		});

		card.addEventListener("click", function (event) {
			if (card.dataset.justDragged === "true") {
				return;
			}

			const interactive = event.target.closest("a, button, input, select, textarea, label");
			if (interactive) {
				return;
			}

			const cardLink = card.querySelector("footer a");
			if (cardLink && cardLink.getAttribute("href")) {
				window.location.href = cardLink.getAttribute("href");
				return;
			}

			const taskId = card.dataset.taskId;
			if (taskId) {
				window.location.href = "/TaskBoard/ViewTask/" + encodeURIComponent(taskId);
				return;
			}

			const taskCode = (card.querySelector("footer")?.textContent || "").trim();
			if (!taskCode) {
				window.location.href = "/TaskBoard/ViewTask";
				return;
			}

			window.location.href = "/TaskBoard/ViewTask/" + encodeURIComponent(taskCode);
		});
	});

	const taskBoards = document.querySelectorAll(".taskboard-page");
	taskBoards.forEach(function (board) {
		const columns = board.querySelectorAll(".taskboard-column");
		const antiForgeryToken = board.querySelector(".js-taskboard-anti-forgery input[name='__RequestVerificationToken']")?.value;
		const statusUpdateUrl = board.dataset.taskStatusUpdateUrl;

		if (columns.length === 0 || !antiForgeryToken || !statusUpdateUrl) {
			return;
		}

		function syncCardStatus(card, statusValue) {
			card.dataset.status = statusValue;
			if (statusValue === "done") {
				card.dataset.completedAt = new Date().toISOString();
			} else {
				delete card.dataset.completedAt;
			}

			renderCardDeadline(card);
			applyBoardFilters(board);
		}

		async function persistCardStatus(taskId, statusValue) {
			const payload = new URLSearchParams();
			payload.append("taskId", taskId);
			payload.append("status", statusValue);
			payload.append("__RequestVerificationToken", antiForgeryToken);

			const response = await fetch(statusUpdateUrl, {
				method: "POST",
				headers: {
					"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
					"X-Requested-With": "XMLHttpRequest"
				},
				body: payload.toString()
			});

			if (!response.ok) {
				throw new Error("Status update failed");
			}

			return response.json();
		}

		columns.forEach(function (column) {
			const targetStatus = column.dataset.columnStatus;
			if (!targetStatus) {
				return;
			}

			column.addEventListener("dragover", function (event) {
				event.preventDefault();
				column.classList.add("is-drop-target");
				if (event.dataTransfer) {
					event.dataTransfer.dropEffect = "move";
				}
			});

			column.addEventListener("dragleave", function (event) {
				if (!column.contains(event.relatedTarget)) {
					column.classList.remove("is-drop-target");
				}
			});

			column.addEventListener("drop", async function (event) {
				event.preventDefault();
				column.classList.remove("is-drop-target");

				const draggedCard = board.querySelector(".task-card.is-dragging");
				if (!draggedCard) {
					return;
				}

				const taskId = draggedCard.dataset.taskId;
				if (!taskId) {
					return;
				}

				const sourceColumn = draggedCard.closest(".taskboard-column");
				const previousStatus = draggedCard.dataset.sourceStatus || getCardStatus(draggedCard);
				if (previousStatus === targetStatus) {
					return;
				}

				column.appendChild(draggedCard);
				syncCardStatus(draggedCard, targetStatus);

				try {
					const payload = await persistCardStatus(taskId, targetStatus);
					if (payload && payload.normalizedStatus) {
						syncCardStatus(draggedCard, payload.normalizedStatus);
					}
				} catch {
					if (sourceColumn) {
						sourceColumn.appendChild(draggedCard);
						syncCardStatus(draggedCard, previousStatus);
					}
				}
			});
		});
	});

	const viewStatusSelect = document.querySelector(".js-task-view-status");
	const viewIndicator = document.querySelector(".js-task-view-deadline");
	const statusDropdown = document.querySelector(".js-status-dropdown");

	if (viewStatusSelect && viewIndicator) {
		if (statusDropdown) {
			const toggle = statusDropdown.querySelector(".js-status-dropdown-toggle");
			const menu = statusDropdown.querySelector(".js-status-dropdown-menu");
			const options = statusDropdown.querySelectorAll(".task-view-status-option");

			if (toggle && menu && options.length > 0) {
				function syncStatusLabel() {
					const selected = viewStatusSelect.options[viewStatusSelect.selectedIndex];
					if (selected) {
						toggle.textContent = selected.textContent;
					}

					options.forEach(function (optionButton) {
						optionButton.classList.toggle("active", optionButton.dataset.value === viewStatusSelect.value);
					});
				}

				toggle.addEventListener("click", function () {
					const isOpen = !menu.hidden;
					menu.hidden = isOpen;
					toggle.setAttribute("aria-expanded", (!isOpen).toString());
				});

				options.forEach(function (optionButton) {
					optionButton.addEventListener("click", function () {
						const value = optionButton.dataset.value || "todo";
						viewStatusSelect.value = value;
						viewStatusSelect.dispatchEvent(new Event("change", { bubbles: true }));
						menu.hidden = true;
						toggle.setAttribute("aria-expanded", "false");
						syncStatusLabel();

						const form = statusDropdown.closest("form");
						if (form) {
							form.submit();
						}
					});
				});

				document.addEventListener("click", function (event) {
					if (!statusDropdown.contains(event.target)) {
						menu.hidden = true;
						toggle.setAttribute("aria-expanded", "false");
					}
				});

				syncStatusLabel();
			}
		}

		function renderViewIndicator() {
			const status = normalizeStatus(viewStatusSelect.value);
			const result = evaluateDeadline(
				viewIndicator.dataset.deadline,
				status,
				viewIndicator.dataset.completedAt
			);

			viewIndicator.textContent = result.text;
			viewIndicator.classList.remove("is-done", "is-late", "is-today", "is-countdown");
			viewIndicator.classList.add(result.state);
		}

		viewStatusSelect.addEventListener("change", function () {
			const status = normalizeStatus(viewStatusSelect.value);

			if (status === "done") {
				viewIndicator.dataset.completedAt = new Date().toISOString();
			} else {
				delete viewIndicator.dataset.completedAt;
			}

			renderViewIndicator();
		});

		renderViewIndicator();
	}

	const richEditors = document.querySelectorAll(".js-rich-editor");
	const userProfileSuccessMessage = document.querySelector(".user-profile-form-success");
	if (userProfileSuccessMessage) {
		setTimeout(function () {
			userProfileSuccessMessage.classList.add("is-hidden");
			setTimeout(function () {
				userProfileSuccessMessage.remove();
			}, 300);
		}, 2200);
	}

	richEditors.forEach(function (container) {
		const editor = container.querySelector(".js-rich-editor-input");
		const hiddenField = container.querySelector(".js-rich-editor-hidden");
		const formatButtons = container.querySelectorAll(".js-rich-format-btn");
		const charCounter = container.querySelector(".js-rich-char-counter");
		const maxCharsValue = Number.parseInt(editor?.dataset.maxChars || "", 10);
		const hasStaticMaxChars = Number.isFinite(maxCharsValue) && maxCharsValue > 0;
		const maxTotalCharsValue = Number.parseInt(container.dataset.maxTotalChars || "", 10);
		const subjectSelector = container.dataset.subjectSelector || "";
		const scopeRoot = container.closest("form") || document;
		const subjectInput = subjectSelector ? scopeRoot.querySelector(subjectSelector) : null;
		const hasDynamicMaxChars = Number.isFinite(maxTotalCharsValue) && maxTotalCharsValue > 0 && Boolean(subjectInput);
		const hasCharLimit = hasStaticMaxChars || hasDynamicMaxChars;

		function getEffectiveMaxChars() {
			if (hasDynamicMaxChars) {
				const subjectLength = ((subjectInput?.value || "").trim()).length;
				const prefixLength = subjectLength > 0 ? 11 + subjectLength : 0;
				return Math.max(0, maxTotalCharsValue - prefixLength);
			}

			if (hasStaticMaxChars) {
				return maxCharsValue;
			}

			return Number.MAX_SAFE_INTEGER;
		}

		if (!editor || formatButtons.length === 0) {
			return;
		}

		function placeCaretAtEnd(element) {
			element.focus();
			const range = document.createRange();
			range.selectNodeContents(element);
			range.collapse(false);
			const selection = window.getSelection();
			selection.removeAllRanges();
			selection.addRange(range);
		}

		function truncateEditorToLimit() {
			if (!hasCharLimit) {
				return;
			}

			const effectiveMaxChars = getEffectiveMaxChars();
			const text = (editor.innerText || "").replace(/\r\n/g, "\n").replace(/\r/g, "\n");
			if (text.length <= effectiveMaxChars) {
				return;
			}

			editor.textContent = text.slice(0, effectiveMaxChars);
			placeCaretAtEnd(editor);
		}

		function syncHiddenValue() {
			if (hiddenField) {
				if (hasCharLimit) {
					hiddenField.value = (editor.innerText || "").replace(/\r\n/g, "\n").replace(/\r/g, "\n");
				} else {
					hiddenField.value = editor.innerHTML;
				}
			}

			if (charCounter && hasCharLimit) {
				const effectiveMaxChars = getEffectiveMaxChars();
				const currentLength = (editor.innerText || "").replace(/\r\n/g, "\n").replace(/\r/g, "\n").length;
				charCounter.textContent = String(currentLength) + "/" + String(effectiveMaxChars);
			}
		}

		formatButtons.forEach(function (button) {
			button.addEventListener("click", function () {
				const command = button.dataset.command;
				if (!command) {
					return;
				}

				editor.focus();
				document.execCommand(command, false);
				syncHiddenValue();
			});
		});

		if (hasCharLimit) {
			editor.addEventListener("beforeinput", function (event) {
				const isDelete = event.inputType && event.inputType.startsWith("delete");
				if (isDelete) {
					return;
				}

				const effectiveMaxChars = getEffectiveMaxChars();
				const currentLength = (editor.innerText || "").length;
				if (currentLength >= effectiveMaxChars) {
					event.preventDefault();
				}
			});

			editor.addEventListener("paste", function (event) {
				event.preventDefault();
				const clipboardText = event.clipboardData?.getData("text/plain") || "";
				const currentText = (editor.innerText || "");
				const remaining = Math.max(0, getEffectiveMaxChars() - currentText.length);
				if (remaining <= 0) {
					return;
				}

				const insertText = clipboardText.slice(0, remaining);
				document.execCommand("insertText", false, insertText);
				truncateEditorToLimit();
				syncHiddenValue();
			});
		}

		editor.addEventListener("input", function () {
			truncateEditorToLimit();
			syncHiddenValue();
		});

		subjectInput?.addEventListener("input", function () {
			truncateEditorToLimit();
			syncHiddenValue();
		});

		truncateEditorToLimit();
		syncHiddenValue();
	});

	const modelValidatedLengthFields = document.querySelectorAll("input[data-val-length-max], textarea[data-val-length-max]");
	modelValidatedLengthFields.forEach(function (field) {
		if (field.hasAttribute("maxlength")) {
			return;
		}

		const maxLength = Number.parseInt(field.getAttribute("data-val-length-max") || "", 10);
		if (Number.isFinite(maxLength) && maxLength > 0) {
			field.setAttribute("maxlength", String(maxLength));
		}
	});

	const limitedTextFields = document.querySelectorAll("input[maxlength]:not([type='hidden']):not([type='file']), textarea[maxlength]");

	limitedTextFields.forEach(function (field) {
		if (field.closest(".js-rich-editor")) {
			return;
		}

		const maxLength = Number.parseInt(field.getAttribute("maxlength") || "", 10);
		if (!Number.isFinite(maxLength) || maxLength <= 0) {
			return;
		}

		const explicitCounter = field.parentElement?.querySelector(".field-char-counter[data-counter-for='" + (field.getAttribute("name") || "") + "']");
		const counter = explicitCounter || document.createElement("div");

		if (!explicitCounter) {
			counter.className = "field-char-counter";
			field.insertAdjacentElement("afterend", counter);
		}

		function syncFieldCounter() {
			if (field.value.length > maxLength) {
				field.value = field.value.slice(0, maxLength);
			}

			counter.textContent = String(field.value.length) + "/" + String(maxLength);
		}

		field.addEventListener("input", syncFieldCounter);
		syncFieldCounter();
	});

	const groupManagePage = document.querySelector(".js-group-manage");
	if (groupManagePage) {
		const toggleAddPanelButton = groupManagePage.querySelector(".js-group-add-toggle");
		const addPanel = groupManagePage.querySelector(".js-group-add-panel");
		const groupManageModal = groupManagePage.querySelector(".group-manage-modal");
		const userSearchInput = groupManagePage.querySelector(".js-group-user-search");
		const availableList = groupManagePage.querySelector(".js-group-available-list");
		const rowsContainer = groupManagePage.querySelector(".js-group-user-rows");
		const toggleSelectionButton = groupManagePage.querySelector(".js-group-toggle-selection");
		const addSelectedButton = groupManagePage.querySelector(".js-group-add-selected");
		const selectionSummary = groupManagePage.querySelector(".js-group-selection-summary");

		if (
			toggleAddPanelButton &&
			addPanel &&
				groupManageModal &&
			userSearchInput &&
			availableList &&
			rowsContainer &&
			toggleSelectionButton &&
			addSelectedButton &&
			selectionSummary
		) {
			function getOptions() {
				return Array.from(availableList.querySelectorAll(".group-manage-option"));
			}

			function getVisibleOptions() {
				return getOptions().filter(function (option) {
					return !option.classList.contains("is-hidden");
				});
			}

			function getCheckedOptions() {
				return getOptions().filter(function (option) {
					const checkbox = option.querySelector(".js-group-user-checkbox");
					return Boolean(checkbox && checkbox.checked);
				});
			}

			function updateSelectionState() {
				const checkedCount = getCheckedOptions().length;
				selectionSummary.textContent = String(checkedCount) + " selected";
				addSelectedButton.disabled = checkedCount === 0;

				const visibleOptions = getVisibleOptions();
				const visibleCheckedCount = visibleOptions.filter(function (option) {
					const checkbox = option.querySelector(".js-group-user-checkbox");
					return Boolean(checkbox && checkbox.checked);
				}).length;

				const shouldUnselect = visibleOptions.length > 0 && visibleCheckedCount === visibleOptions.length;
				toggleSelectionButton.textContent = shouldUnselect ? "Unselect all" : "Select all";
			}

			function applyUserFilter() {
				const query = (userSearchInput.value || "").trim().toLowerCase();
				getOptions().forEach(function (option) {
					const candidateName = option.dataset.name || "";
					option.classList.toggle("is-hidden", query !== "" && !candidateName.includes(query));
				});
			}

			function syncAddPanelState() {
				groupManageModal.classList.toggle("is-add-open", !addPanel.hidden);
			}

			toggleAddPanelButton.addEventListener("click", function () {
				const shouldOpen = addPanel.hidden;
				addPanel.hidden = !shouldOpen;
				syncAddPanelState();

				if (shouldOpen) {
					userSearchInput.focus();
				}
			});

			userSearchInput.addEventListener("input", function () {
				applyUserFilter();
				updateSelectionState();
			});

			availableList.addEventListener("change", function (event) {
				if (event.target && event.target.classList.contains("js-group-user-checkbox")) {
					updateSelectionState();
				}
			});

			toggleSelectionButton.addEventListener("click", function () {
				const visibleOptions = getVisibleOptions();
				const allVisibleSelected =
					visibleOptions.length > 0 &&
					visibleOptions.every(function (option) {
						const checkbox = option.querySelector(".js-group-user-checkbox");
						return Boolean(checkbox && checkbox.checked);
					});

				visibleOptions.forEach(function (option) {
					const checkbox = option.querySelector(".js-group-user-checkbox");
					if (checkbox) {
						checkbox.checked = !allVisibleSelected;
					}
				});

				updateSelectionState();
			});

			addSelectedButton.addEventListener("click", function () {
				if (addSelectedButton.dataset.serverSubmit === "true") {
					return;
				}

				const selectedOptions = getCheckedOptions();
				if (selectedOptions.length === 0) {
					return;
				}

				selectedOptions.forEach(function (option) {
					const checkbox = option.querySelector(".js-group-user-checkbox");
					if (!checkbox) {
						return;
					}

					const name = checkbox.dataset.userName || "Unknown user";
					const userId = checkbox.value || "";

					const row = document.createElement("div");
					row.className = "group-manage-row";
					row.dataset.userId = userId;
					row.innerHTML =
						"<span></span><span>Student</span><button type='button' class='group-manage-remove-btn'>Remove</button>";
					row.querySelector("span").textContent = name;

					rowsContainer.appendChild(row);
					option.remove();
				});

				userSearchInput.value = "";
				applyUserFilter();
				updateSelectionState();
			});

			applyUserFilter();
			updateSelectionState();
			syncAddPanelState();
		}
	}

	const userSettings = document.querySelector(".js-user-settings");
	const profilePhotoForm = document.querySelector(".js-profile-photo-form");
	if (profilePhotoForm) {
		const pickButton = profilePhotoForm.querySelector(".js-profile-photo-btn");
		const fileInput = profilePhotoForm.querySelector(".js-profile-photo-input");
		const actions = profilePhotoForm.querySelector(".js-profile-photo-actions");
		const cancelButton = profilePhotoForm.querySelector(".js-profile-photo-cancel");
		const previewImage = document.querySelector(".js-profile-photo-image");
		let previewObjectUrl = null;

		if (pickButton && fileInput && actions && cancelButton && previewImage) {
			function clearPreview() {
				if (previewObjectUrl) {
					URL.revokeObjectURL(previewObjectUrl);
					previewObjectUrl = null;
				}

				fileInput.value = "";
				actions.hidden = true;
				previewImage.src = previewImage.dataset.originalSrc || previewImage.src;
			}

			pickButton.addEventListener("click", function () {
				fileInput.click();
			});

			fileInput.addEventListener("change", function () {
				if (fileInput.files && fileInput.files.length > 0) {
					const selectedFile = fileInput.files[0];
					if (previewObjectUrl) {
						URL.revokeObjectURL(previewObjectUrl);
					}

					previewObjectUrl = URL.createObjectURL(selectedFile);
					previewImage.src = previewObjectUrl;
					actions.hidden = false;
				}
			});

			cancelButton.addEventListener("click", function () {
				clearPreview();
			});

			profilePhotoForm.addEventListener("submit", function () {
				actions.hidden = true;
			});
		}
	}

	if (userSettings) {
		const toggle = userSettings.querySelector(".js-user-settings-toggle");
		const hiddenValue = userSettings.querySelector(".js-user-settings-value");
		const reminderOffset = userSettings.querySelector(".js-user-reminder-offset");
		const reminderType = userSettings.querySelector(".js-user-reminder-type");

		if (toggle && hiddenValue) {
			function syncUserSettingsState(isEnabled) {
				userSettings.classList.toggle("is-enabled", isEnabled);
				toggle.setAttribute("aria-pressed", isEnabled ? "true" : "false");
				hiddenValue.value = isEnabled ? "true" : "false";

				if (reminderOffset) {
					reminderOffset.disabled = !isEnabled;
				}
				if (reminderType) {
					reminderType.disabled = !isEnabled;
				}
			}

			const startEnabled = (userSettings.dataset.enabled || "true") === "true";
			syncUserSettingsState(startEnabled);

			toggle.addEventListener("click", function () {
				const currentState = hiddenValue.value === "true";
				syncUserSettingsState(!currentState);
			});
		}
	}

	const storageRoot = document.querySelector(".js-storage-files");
	if (storageRoot) {
		const searchInput = storageRoot.querySelector(".js-storage-search");
		const scopeFilter = storageRoot.querySelector(".js-storage-scope-filter");
		const fileRows = Array.from(document.querySelectorAll(".js-storage-row[data-storage-row='true']"));
		const emptyFilteredRow = document.querySelector(".js-storage-empty-result");

		if (searchInput && scopeFilter && fileRows.length > 0) {
			function applyStorageFilters() {
				const query = (searchInput.value || "").trim().toLowerCase();
				const selectedScope = (scopeFilter.value || "all").toLowerCase();
				let visibleCount = 0;

				fileRows.forEach(function (row) {
					const name = (row.dataset.storageName || "").toLowerCase();
					const scope = (row.dataset.storageScope || "personal").toLowerCase();
					const matchesSearch = query === "" || name.includes(query);
					const matchesScope = selectedScope === "all" || scope === selectedScope;
					const isVisible = matchesSearch && matchesScope;

					row.hidden = !isVisible;
					if (isVisible) {
						visibleCount += 1;
					}
				});

				if (emptyFilteredRow) {
					emptyFilteredRow.hidden = visibleCount > 0;
				}
			}

			searchInput.addEventListener("input", applyStorageFilters);
			scopeFilter.addEventListener("change", applyStorageFilters);
			applyStorageFilters();
		}
	}
});
