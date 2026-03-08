using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Application.Models;

namespace Application.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("User opened the {PageName} page at {Time}", "Index", DateTime.Now);
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("User navigated to the {PageName} page", "Privacy");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("An error occurred. RequestId: {RequestId}", requestId);
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}