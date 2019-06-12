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

        private readonly IPlanRepository _planRepository;

        public ManageController(ILoggerManager loggerManager, IDictionaryRepository dictionaryRepository, IPlanRepository planRepository) : base(loggerManager)
        {
            _dictionaryRepository = dictionaryRepository;

            _planRepository = planRepository;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPositionList()
        {
            var positions = _dictionaryRepository.GetPositions(false);

            ViewData["Title"] = "Positions";
            ViewData["HasPosition"] = true;

            return View("DictionaryList", positions);
        }

        [HttpGet]
        public IActionResult GetValueList()
        {
            var values = _dictionaryRepository.GetValues(false);

            ViewData["Title"] = "Values";
            ViewData["HasValue"] = true;

            return View("DictionaryList", values);
        }

        [HttpGet]
        public IActionResult GetStakeholderCategoryList()
        {
            var stakeholderCategories = _dictionaryRepository.GetStakeholderCategories(false);

            ViewData["Title"] = "Stakeholder Categories";
            ViewData["HasStakeholderCategory"] = true;

            return View("DictionaryList", stakeholderCategories);
        }

        [HttpGet]
        public IActionResult GetStakeholderCriterionList()
        {
            var stakeholderCriteria = _dictionaryRepository.GetStakeholderCriteria(false);

            ViewData["Title"] = "Stakeholder Criteria";
            ViewData["HasStakeholderCriteria"] = true;

            return View("DictionaryList", stakeholderCriteria);
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
                result = _dictionaryRepository.Delete(id, HttpContext.GetUserId());
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult ActivateDictionary(int id)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                result = _dictionaryRepository.Activate(id, HttpContext.GetUserId());
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult DisactivateDictionary(int id)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                result = _dictionaryRepository.Disactivate(id, HttpContext.GetUserId());
            }

            return new JsonResult(new { result });
        }

        [HttpGet]
        public IActionResult GetIntroductionList()
        {
            return View("IntroductionList",_planRepository.GetIntroductions());
        }
    }
}
