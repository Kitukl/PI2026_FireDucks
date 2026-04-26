using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Application.Controllers;

public class HomeController : Controller
{
    [Authorize]
    [HttpGet("/Home/Index")]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }
}
