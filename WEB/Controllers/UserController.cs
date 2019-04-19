using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class UserController : AbstractController
    {
        public UserController(ILoggerManager loggerManager) : base(loggerManager)
        {
        }

        public IActionResult GetProfile(int userId)
        {
            return View("Profile");
        }

        public IActionResult GetMyProfile()
        {
            return RedirectToAction("GetProfile", new { userId = 0 });
        }

        public IActionResult GetActivity(int userId)
        {
            return View("Activity");
        }

        public IActionResult GetMyActivity()
        {
            return RedirectToAction("GetActivity", new { userId = 0 });
        }
    }
}