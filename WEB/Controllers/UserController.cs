using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class UserController : AbstractController
    {
        private readonly IUserRepository _userRepository;

        private readonly IEmailService _emailService;

        private readonly IPlanRepository _planRepository;

        public UserController(ILoggerManager loggerManager,
            IUserRepository userRepository,
            IEmailService emailService,
            IPlanRepository planRepository) : base(loggerManager)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _planRepository = planRepository;
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

        [HttpGet]
        public IActionResult AddUserToPlan()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddExistingUser(ExistingUserAddDTO existingUser)
        {
            if (ModelState.IsValid)
            {
                var result = _planRepository.AddUserToPlan(existingUser.Id, existingUser.PlanId, existingUser.PositionId);

                if(result)
                {
                    Response.StatusCode = StatusCodes.Status201Created;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User is already added to the planning or something went wrong");
                }
            }

            return PartialView("_AddExistingUser");
        }

        [HttpPost]
        public IActionResult AddNewUser(NewUserDTO newUser, int? planId)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.FindByCondition(u => u.Email == newUser.Email).Any())
                {
                    ModelState.AddModelError(nameof(newUser.Email), "An user with the email exists");

                    return PartialView("_AddNewUser");
                }

                newUser.Password = _userRepository.GeneratePassword();
                var user = _userRepository.AddNew(newUser);

                if (planId.HasValue)
                {
                    _planRepository.AddUserToPlan(user.Id, planId.Value, newUser.PositionId);
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Adding a new user failed");

                    return PartialView("_AddNewUser");
                }

                if(!_emailService.SendPasswordToUser(newUser.Password, user))
                {
                    ModelState.AddModelError(string.Empty, "Email was not sent to the user");

                    return PartialView("_AddNewUser");
                }

                Response.StatusCode = StatusCodes.Status201Created;
            }

            return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");

        }
    }
}