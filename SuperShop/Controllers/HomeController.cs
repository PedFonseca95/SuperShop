﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperShop.Models;
using System.Diagnostics;

namespace SuperShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")] // Quando o erro vier daqui
        public IActionResult Error404()
        {
            return View();
        }
    }
}
