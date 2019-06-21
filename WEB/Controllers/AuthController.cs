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
            _loggerManager.Info("GET Login is requested");

            if (User.Identity.IsAuthenticated)
            {
                _loggerManager.Warn("User is already authenticated and redirected to home");
                return RedirectToAction("Index", "Home", null);
            }

            var login = new UserLoginDTO { ReturnUrl = returnUrl };

            _loggerManager.Info("Login() returned a result");

            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(UserLoginDTO login)
        {
            _loggerManager.Info("POST Login is requested");

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
                    _loggerManager.Warn("Authentication attempt failed");
                }
            }
            else
            {
                _loggerManager.Warn("Authentication values are invalid");
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
            _loggerManager.Info("GET ForgotPassword is requested");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            _loggerManager.Info("POST ForgotPassword is requested");

            if (ModelState.IsValid)
            {
                var user = _userRepository.GetUserByEmail(forgotPassword.Email);
                if (user == null)
                {
                    _loggerManager.Warn("ForgotPassword - an user does not exist");

                    ModelState.AddModelError(string.Empty, "An user with the email does not exist");

                    return View();
                }

                var token = _userRepository.GetRecoveryToken(forgotPassword.Email);
                var url = _linkGenerator.GetUriByAction(HttpContext, "RecoverPassword", "Auth", new { token });

                if (!_emailService.SendPasswordRecoveryInfo(url, user))
                {
                    _loggerManager.Warn("ForgotPassword - It was unable to send a recovery email to the address");

                    ModelState.AddModelError(nameof(forgotPassword.Email), "It was unable to send a recovery email to the address");

                    return View();
                }

                var result = new ResultVM
                {
                    Title = "Successfully sent",
                    Text = $"Recovery link was sent to {forgotPassword.Email}. It will be valid within next 24 hours"
                };

                _loggerManager.Info("ForgotPassword - successfully sent password recovery info");

                return View("Success", result);

            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecoverPassword(string token)
        {
            _loggerManager.Info("GET RecoverPassword is requested");

            if (!_userRepository.ValidateToken(token))
            {
                _loggerManager.Warn("Token is not valid");

                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            _loggerManager.Info("Successfully returned password recovery page");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult RecoverPassword(RecoverPasswordDTO recoverPassword)
        {
            _loggerManager.Info("POST RecoverPassword is requested");

            if (ModelState.IsValid)
            {
                if (_userRepository.RecoverPassword(recoverPassword,HttpContext.GetUserId()))
                {
                    var result = new ResultVM
                    {
                        Title = "Successfully recovered",
                        Text = "New password was set. You can log in now."
                    };

                    _loggerManager.Info("Password recovered successfully");

                    return View("Success", result);
                }
                else
                {
                    _loggerManager.Error("Password recovery failed");

                    ModelState.AddModelError(string.Empty, "An error occured during updating password");
                }
            }
            else
            {
                _loggerManager.Warn("Password recovery values are invalid");
            }

            return View(); ;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            _loggerManager.Info("AccessDenied page returned");

            return View();
        }

        [HttpPost]
        [ResponseCache(NoStore = true)]
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