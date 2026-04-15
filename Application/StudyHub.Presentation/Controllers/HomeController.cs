using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

public class HomeController : Controller
{
    [HttpGet("/Home/Index")]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
