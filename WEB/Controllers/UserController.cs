using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.Constants;
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
        public IActionResult AddExistingUserToPlan(AddUserToPlanDTO addUserToPlan, int planId)
        {
            if (planId <= 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
            }

            var existingUser = addUserToPlan.ExistingUser;

            if (TryValidateModel(existingUser))
            {
                var result = _planRepository.AddUserToPlan(existingUser.Id.Value, planId, existingUser.PositionId.Value);

                if (result)
                {
                    Response.StatusCode = StatusCodes.Status201Created;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User is already added to the planning or something went wrong");
                }
            }

            return PartialView("~/Views/User/Partials/_AddExistingUser.cshtml");
        }

        [HttpPost]
        public IActionResult AddNewUserToPlan(AddUserToPlanDTO addUserToPlan, int planId)
        {
            if (planId <= 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
            }

            var newUser = addUserToPlan.NewUser;

            if (TryValidateModel(newUser))
            {

                if (_userRepository.FindByCondition(u => u.Email == newUser.Email).Any())
                {
                    ModelState.AddModelError("NewUser.Email", "An user with the email exists");

                    return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
                }

                newUser.Password = _userRepository.GeneratePassword();
                newUser.Role = Roles.Employee;
                var user = _userRepository.AddNew(newUser);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Adding a new user failed");

                    return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
                }

                if (!_emailService.SendPasswordToUser(newUser.Password, user))
                {
                    ModelState.AddModelError(string.Empty, "Email was not sent to the user");

                    return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
                }

                if (!_planRepository.AddUserToPlan(user.Id, planId, newUser.PositionId.Value))
                {
                    ModelState.AddModelError(string.Empty, "User was not added to the planning team due ");

                    return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
                }


                Response.StatusCode = StatusCodes.Status201Created;
            }

            return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");

        }

        [HttpPost]
        public IActionResult RemoveUserFromPlan(int userId, int planId)
        {
            if (userId <= 0 && planId <= 0)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var result = _planRepository.RemoveUserFromPlan(userId, planId);

            return new JsonResult(new { result });

        }
    }
}