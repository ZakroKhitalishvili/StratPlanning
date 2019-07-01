using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Web.Controllers
{
    /// <summary>
    /// Base class for all controllers. Common properties and methods should be added here
    /// </summary>
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