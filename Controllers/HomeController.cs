using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kanban.Controllers.Helpers;
using tl2_tp10_2023_Santincho.Models;

namespace tl2_tp10_2023_Santincho.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (LoginHelper.IsLogged(HttpContext)) return View();
        return RedirectToAction("Index", "Login");
    }

    public IActionResult Privacy()
    {
        if (LoginHelper.IsLogged(HttpContext)) return View();
        return RedirectToAction("Index", "Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        string? errorMessage = TempData["ErrorMessage"] as string;
        string? stackTrace = TempData["StackTrace"] as string;

        // Puedes pasar los datos a la vista si es necesario
        ViewData["ErrorMessage"] = errorMessage;
        ViewData["StackTrace"] = stackTrace;
        
        return View();
        //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
