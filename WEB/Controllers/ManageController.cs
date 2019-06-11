using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class ManageController : AbstractController
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public ManageController(ILoggerManager loggerManager, IDictionaryRepository dictionaryRepository) : base(loggerManager)
        {
            _dictionaryRepository = dictionaryRepository;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPositionList()
        {
            var positions = _dictionaryRepository.GetPositions().Select(x => new DictionaryDTO { Id = x.Id, Title = x.Title, HasPosition = true });

            ViewData["Title"] = "Positions";
            ViewData["HasPosition"] = true;

            return View("DictionaryList", positions);
        }

        [HttpPost]
        public IActionResult CreateDictionary(DictionaryDTO dictionary)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                var result = _dictionaryRepository.Create(dictionary, HttpContext.GetUserId());

                if (result)
                {
                    Response.StatusCode = StatusCodes.Status201Created;
                }
            }

            return PartialView("~/Views/Manage/Partials/_NewDictionary.cshtml");
        }

        [HttpPost]
        public IActionResult DeleteDictionary(int id)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                result = _dictionaryRepository.Delete(id);
            }

            return new JsonResult(new { result });
        }
    }
}
