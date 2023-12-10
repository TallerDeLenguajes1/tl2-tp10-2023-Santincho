﻿using System.Diagnostics;
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
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
