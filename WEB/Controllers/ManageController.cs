using System;
using System.Collections.Generic;
using System.IO;
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
using Web.Helpers;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class ManageController : AbstractController
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        private readonly IPlanRepository _planRepository;

        private readonly IFileRepository _fileRepository;

        public ManageController(ILoggerManager loggerManager, IDictionaryRepository dictionaryRepository, IPlanRepository planRepository, IFileRepository fileRepository) : base(loggerManager)
        {
            _dictionaryRepository = dictionaryRepository;

            _planRepository = planRepository;

            _fileRepository = fileRepository;
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
            return RedirectToAction("GetIntroduction", new { stepIndex = Steps.Predeparture });
        }

        [HttpGet]
        public IActionResult GetIntroduction(string stepIndex)
        {

            if (!_planRepository.GetStepList().Contains(stepIndex))
            {
                return BadRequest();
            }

            var introduction = _planRepository.GetIntroduction(stepIndex);

            if (introduction == null)
            {
                introduction = new IntroductionDTO { Step = stepIndex };
            }

            return View("Introduction", introduction);
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public IActionResult UploadIntroduction(IntroductionDTO introduction)
        {
            var files = HttpContext.Request.Form.Files;

            if (files == null || files.Count != 1 )
            {
                ModelState.AddModelError(string.Empty, "A file is not chosen");
                
                return View("Introduction", introduction);
            }

            if (!_planRepository.GetStepList().Contains(introduction.Step))
            {
                return BadRequest();
            }

            var userId = HttpContext.GetUserId();

            var uploadRelPath = UploadHelper.Upload(files[0]);

            var fileDto = _fileRepository.CreateNewFile(Path.GetFileNameWithoutExtension(files[0].FileName), Path.GetExtension(files[0].FileName), uploadRelPath, userId);

            var result = false;

            if (fileDto != null)
            {
                var oldVideo = _planRepository.GetIntroduction(introduction.Step)?.Video;
                result = _planRepository.UpdateIntroduction(introduction.Step, fileDto.Id, userId);
                if (result && oldVideo != null)
                {
                    UploadHelper.Delete(oldVideo.Path);
                    _fileRepository.Delete(oldVideo.Id);
                }
            }

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong due to a server");
            }

            return RedirectToAction("GetIntroduction", new { stepIndex = introduction.Step });
        }

    }
}
