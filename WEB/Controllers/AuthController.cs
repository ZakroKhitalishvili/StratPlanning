using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Application.Interfaces.Services;
using Web.ViewModels;
using Microsoft.AspNetCore.Routing;
using Web.Extensions;
using System;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class AuthController : AbstractController
    {
        private readonly IUserRepository _userRepository;

        private readonly IEmailService _emailService;

        private readonly LinkGenerator _linkGenerator;

        public AuthController(IUserRepository userRepository, ILoggerManager loggerManager, IEmailService emailService, LinkGenerator linkGenerator) : base(loggerManager)
        {
            _userRepository = userRepository;

            _emailService = emailService;

            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", null);
            }

            var login = new UserLoginDTO { ReturnUrl = returnUrl };

            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(UserLoginDTO login)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.TryAuthentication(login.Email.Trim(), login.Password, out UserDTO user))
                {
                    _loggerManager.Info("Authentication attempt was successfull");

                    HttpContext.LogIn(user, login.RememberMe);

                    if (string.IsNullOrEmpty(login.ReturnUrl))
                    {
                        login.ReturnUrl = "/";
                    }

                    return Redirect(login.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email or password is wrong");
                    _loggerManager.Info("Authentication attempt failed");
                }
            }
            else
            {
                _loggerManager.Info("Authentication values are invalid");
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.LogOut();

            _loggerManager.Info("Logged out");

            return View("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.GetUserByEmail(forgotPassword.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "An user with the email does not exist");

                    return View();
                }

                var token = _userRepository.GetRecoveryToken(forgotPassword.Email);
                var url = _linkGenerator.GetUriByAction(HttpContext, "RecoverPassword", "Auth", new { token });

                if (!_emailService.SendPasswordRecoveryInfo(url, user))
                {
                    ModelState.AddModelError(nameof(forgotPassword.Email), "It was unable to send a recovery email to the address");

                    return View();
                }

                var result = new ResultVM
                {
                    Title = "Successfully sent",
                    Text = $"Recovery link was sent to {forgotPassword.Email}. It will be valid within next 24 hours"
                };

                return View("Success", result);

            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecoverPassword(string token)
        {
            if (!_userRepository.ValidateToken(token))
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult RecoverPassword(RecoverPasswordDTO recoverPassword)
        {
            if (ModelState.IsValid)
            {

                if (_userRepository.RecoverPassword(recoverPassword))
                {
                    var result = new ResultVM
                    {
                        Title = "Successfully recovered",
                        Text = "New password was set. You can log in now."
                    };

                    return View("Success", result);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occured during updating password");
                }
            }

            return View(); ;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ping()
        {
            var expiration = await HttpContext.GetExpiration();

            int expirationSeconds = 0;

            if (expiration.HasValue)
            {
                expirationSeconds = (int)expiration.Value.Subtract(DateTime.Now).TotalSeconds;
            }

            return Json(new { expirationSeconds });

        }
    }
}