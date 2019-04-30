using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Web.Controllers
{
    [Authorize]
    public abstract class AbstractController : Controller
    {
        protected readonly ILoggerManager _loggerManager;

        public AbstractController(ILoggerManager loggerManager)
        {
            _loggerManager = loggerManager;
        }
    }
}