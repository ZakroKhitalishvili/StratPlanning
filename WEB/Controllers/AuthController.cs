using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Constants;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Application.Interfaces.Services;
using Web.ViewModels;
using Microsoft.AspNetCore.Routing;

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
            var login = new UserLoginDTO { ReturnUrl = returnUrl };

            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO login)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.TryAuthentication(login.Email.Trim(), login.Password, out UserDTO user))
                {
                    _loggerManager.Info("Authentication attempt was successfull");
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(CustomClaimTypes.Position, user.Position?.Title??string.Empty),
                        new Claim(CustomClaimTypes.Id, user.Id.ToString())
                };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    DateTimeOffset? expiresUtc = null;
                    bool isPersistent = false;
                    if (login.RememberMe)
                    {
                        expiresUtc = DateTimeOffset.UtcNow.AddDays(7);
                        isPersistent = true;
                    }

                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        ExpiresUtc = expiresUtc,
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = isPersistent,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = login.ReturnUrl
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);

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
                    ModelState.AddModelError(nameof(forgotPassword.Email), "It was unable to send a recovery email to the email");

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
    }
}