using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

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

        public IActionResult GetProfile(int? userId)
        {
            if (!userId.HasValue)
            {
                return RedirectToAction("GetMyProfile");
            }

            var user = _userRepository.GetUserById(userId.Value);

            return View("Profile", new UserProfileDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            });
        }

        public IActionResult GetMyProfile()
        {
            var id = HttpContext.GetUserId();

            return RedirectToAction("GetProfile", new { userId = id });
        }

        public IActionResult GetActivity(int? userId)
        {
            return View("Activity");
        }

        public IActionResult GetMyActivity()
        {
            var id = HttpContext.GetUserId();

            return RedirectToAction("GetActivity", new { userId = id });
        }

        [HttpGet]
        public IActionResult AddUserToPlan()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult AddExistingUserToPlan(AddUserToPlanDTO addUserToPlan, int planId)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

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
        [Authorize(Roles = Roles.Admin)]
        public IActionResult AddNewUserToPlan(AddUserToPlanDTO addUserToPlan, int planId)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

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
        [Authorize(Roles = Roles.Admin)]
        public IActionResult AddExistingExternalUserToStep(AddResponsibleUserToStepDTO addExternalUserToStep)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

            if (string.IsNullOrEmpty(addExternalUserToStep.Step))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
            }

            var existingUser = addExternalUserToStep.ExistingResponsibleUser;

            if (TryValidateModel(existingUser))
            {
                var result = _planRepository.AddUserToPlanPlanStep(existingUser.Id.Value, addExternalUserToStep.PlanId, addExternalUserToStep.Step);

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
        [Authorize(Roles = Roles.Admin)]
        public IActionResult AddNewExternalUserToStep(AddResponsibleUserToStepDTO addExternalUserToStep)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

            if (string.IsNullOrEmpty(addExternalUserToStep.Step) && addExternalUserToStep.PlanId <= 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
            }

            var newExternaluser = addExternalUserToStep.NewResponsibleUser;

            if (TryValidateModel(newExternaluser))
            {

                if (_userRepository.FindByCondition(u => u.Email == newExternaluser.Email).Any())
                {
                    ModelState.AddModelError("NewUser.Email", "An user with the email exists");

                    return PartialView("~/Views/User/Partials/_AddNewUser.cshtml");
                }
                var newUser = new NewUserDTO
                {
                    Email = newExternaluser.Email,
                    FirstName = newExternaluser.FirstName,
                    LastName = newExternaluser.LastName
                };

                newUser.Password = _userRepository.GeneratePassword();
                newUser.Role = Roles.External;
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

                if (!_planRepository.AddUserToPlanPlanStep(user.Id, addExternalUserToStep.PlanId, addExternalUserToStep.Step))
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

        [HttpPost]
        public IActionResult UpdateProfile(UserProfileDTO userProfile)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                userProfile.Id = HttpContext.GetUserId();

                if (_userRepository.FindByCondition(u => u.Email == userProfile.Email && u.Id != userProfile.Id).Any())
                {
                    ModelState.AddModelError(nameof(userProfile.Email), "An user with the email existes");

                    return PartialView("~/Views/User/Partials/_UserProfileDetails.cshtml");
                }

                if (!_userRepository.UpdateProfile(userProfile))
                {
                    ModelState.AddModelError(string.Empty, "Profile is not updated");

                    return PartialView("~/Views/User/Partials/_UserProfileDetails.cshtml");
                }

                var user = _userRepository.GetUserById(userProfile.Id);

                HttpContext.UpdateUser(user);

                Response.StatusCode = StatusCodes.Status200OK;
            }

            return PartialView("~/Views/User/Partials/_UserProfileDetails.cshtml");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordDTO changePassword)
        {

            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                var id = HttpContext.GetUserId();
                var email = HttpContext.GetUserEmail();


                if (!_userRepository.TryAuthentication(email, changePassword.Password, out UserDTO user))
                {
                    ModelState.AddModelError(nameof(changePassword.Password), "Authentication failed: password is wrong");

                    return PartialView("~/Views/User/Partials/_ChangePassword.cshtml");
                }

                changePassword.Id = id;

                if (!_userRepository.ChangePassword(changePassword))
                {
                    ModelState.AddModelError(string.Empty, "Password was not updated");

                    return PartialView("~/Views/User/Partials/_ChangePassword.cshtml");
                }

                Response.StatusCode = StatusCodes.Status200OK;

            }

            return PartialView("~/Views/User/Partials/_ChangePassword.cshtml");
        }

    }
}