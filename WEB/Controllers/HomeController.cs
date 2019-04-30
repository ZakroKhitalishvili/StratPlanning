using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;


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
            return View();
        }
    }
}
