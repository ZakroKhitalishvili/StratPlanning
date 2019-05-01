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

namespace Web.Controllers
{
    public class AuthController : AbstractController
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository, ILoggerManager loggerManager) : base(loggerManager)
        {
            _userRepository = userRepository;
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
                if (_userRepository.TryAuthentication(login.Email, login.Password, out UserDTO user))
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

                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = login.RememberMe,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        RedirectUri = login.ReturnUrl
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
        public IActionResult ForgotPassword(string email)
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}