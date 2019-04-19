using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : AbstractController
    {
        public HomeController(ILoggerManager loggerManager):base(loggerManager)
        {
        }

        public IActionResult Index()
        {
            _loggerManager.Info("Home.Index was invoked");

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
    }
}
