using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

public class SchedulePageController : Controller
{
    [HttpGet("/Schedule")]
    public IActionResult Schedule()
    {
        return RedirectToAction("MySchedule", "Schedule");
    }
}
