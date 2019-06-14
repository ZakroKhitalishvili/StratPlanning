using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using System;
using Microsoft.AspNetCore.Diagnostics;

namespace Web.Controllers
{
    public class HomeController : AbstractController
    {
        public HomeController(ILoggerManager loggerManager) : base(loggerManager)
        {
        }

        public IActionResult Index()
        {
            _loggerManager.Info("Home.Index was requested");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (feature != null)
            {
                _loggerManager.Error("Uncaught exception was thrown.", feature.Error);
            }

            return View();
        }
    }
}
