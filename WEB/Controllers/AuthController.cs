using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces;

namespace Web.Controllers
{
    public class AuthController : AbstractController
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository, ILoggerManager loggerManager) : base(loggerManager)
        {
            _userRepository = userRepository;
        }

        public IActionResult Login(string returnUrl)
        {
            var login = new UserLoginDTO { ReturnUrl = returnUrl };

            return View(login);
        }

        [HttpPost]
        public IActionResult Login(UserLoginDTO login)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.AuthenticateUser(login.Email, login.Password))
                {
                    _loggerManager.Info("Authentication attempt was successfull");
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"Email or password is wrong");
                    _loggerManager.Info("Authentication attempt failed");
                }
            }
            else
            {
                _loggerManager.Info("Authentication values are invalid");
            }

            return View();
        }
    }
}