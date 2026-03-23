using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using StudyHub.Domain.Entities;
using System.Security.Claims;

namespace Application.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user != null)
            {
                ViewBag.FullName = $"{user.Name} {user.Surname}";
            }
        }
        else
        {
            ViewBag.FullName = "Гість";
            _logger.LogInformation("Anonymous user opened the Index page");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("/myprofile")]
    [HttpGet("/UserProfile")]
    public async Task<IActionResult> UserProfile()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.FullName = $"{user.Name} {user.Surname}".Trim();
                ViewBag.PhotoUrl = string.IsNullOrWhiteSpace(user.PhotoUrl)
                    ? Url.Content("~/images/no-photo.png")
                    : user.PhotoUrl;
            }
        }

        ViewBag.FullName ??= "Name Surname";
        ViewBag.PhotoUrl ??= Url.Content("~/images/no-photo.png");

        return View();
    }

    [HttpGet("/TaskBoard")]
    public IActionResult TaskBoard()
    {
        return View();
    }

    [HttpGet("/TaskBoard/Create")]
    [HttpGet("/TaskBoard/CreateTask")]
    public IActionResult TaskBoardCreate()
    {
        return View();
    }

    [HttpGet("/TaskBoard/ViewTask/{taskCode?}")]
    public IActionResult TaskBoardViewTask(string? taskCode)
    {
        ViewBag.TaskCode = string.IsNullOrWhiteSpace(taskCode) ? "A-3" : taskCode;
        return View();
    }

    [HttpGet("/TaskBoard/ReviewGroup")]
    public IActionResult TaskBoardReviewGroup()
    {
        return View();
    }

    [HttpGet("/Storage")]
    public IActionResult Storage()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}