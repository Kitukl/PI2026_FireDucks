using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[Route("[controller]")]
public class UserPlatform : Controller
{
    [HttpGet("Index")]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
