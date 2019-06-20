﻿using System;
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

        private readonly ISettingRepository _settingRepository;

        public ManageController(ILoggerManager loggerManager,
                                IDictionaryRepository dictionaryRepository,
                                IPlanRepository planRepository,
                                IFileRepository fileRepository,
                                ISettingRepository settingRepository) : base(loggerManager)
        {
            _dictionaryRepository = dictionaryRepository;

            _planRepository = planRepository;

            _fileRepository = fileRepository;

            _settingRepository = settingRepository;
        }

        [HttpGet]
        public IActionResult GetPositionList()
        {
            _loggerManager.Info($"GetPositionList is requested");

            var positions = _dictionaryRepository.GetPositions(false);

            ViewData["Title"] = "Positions";
            ViewData["HasPosition"] = true;

            return View("DictionaryList", positions);
        }

        [HttpGet]
        public IActionResult GetValueList()
        {
            _loggerManager.Info($"GetValueList is requested");

            var values = _dictionaryRepository.GetValues(false);

            ViewData["Title"] = "Values";
            ViewData["HasValue"] = true;

            return View("DictionaryList", values);
        }

        [HttpGet]
        public IActionResult GetStakeholderCategoryList()
        {
            _loggerManager.Info($"GetStakeholderCategoryList is requested");

            var stakeholderCategories = _dictionaryRepository.GetStakeholderCategories(false);

            ViewData["Title"] = "Stakeholder Categories";
            ViewData["HasStakeholderCategory"] = true;

            return View("DictionaryList", stakeholderCategories);
        }

        [HttpGet]
        public IActionResult GetStakeholderCriterionList()
        {
            _loggerManager.Info($"GetStakeholderCriterionList is requested");

            var stakeholderCriteria = _dictionaryRepository.GetStakeholderCriteria(false);

            ViewData["Title"] = "Stakeholder Criteria";
            ViewData["HasStakeholderCriteria"] = true;

            return View("DictionaryList", stakeholderCriteria);
        }

        [HttpPost]
        public IActionResult CreateDictionary(DictionaryDTO dictionary)
        {
            _loggerManager.Info($"CreateDictionary for {dictionary.Title} is requested");

            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                var result = _dictionaryRepository.Create(dictionary, HttpContext.GetUserId());

                if (result)
                {
                    Response.StatusCode = StatusCodes.Status201Created;
                    _loggerManager.Info($"CreateDictionary for {dictionary.Title} successfully created a dictionary");
                }
                else
                {
                    _loggerManager.Warn($"CreateDictionary for {dictionary.Title} was unable to create a dictionary");
                }
            }
            else
            {
                _loggerManager.Warn($"CreateDictionary for {dictionary.Title} was invalid");
            }

            return PartialView("~/Views/Manage/Partials/_NewDictionary.cshtml");
        }

        [HttpPost]
        public IActionResult DeleteDictionary(int id)
        {
            _loggerManager.Info($"DeleteDictionary({id}) was requested");

            var result = false;

            if (ModelState.IsValid)
            {
                result = _dictionaryRepository.Delete(id, HttpContext.GetUserId());

                if (result)
                {
                    _loggerManager.Info($"DeleteDictionary({id}) successfully deleted");
                }
                else
                {
                    _loggerManager.Warn($"DeleteDictionary({id}) was unable to delete");
                }
            }
            else
            {
                _loggerManager.Warn($"DeleteDictionary({id}) was invalid");
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult ActivateDictionary(int id)
        {
            _loggerManager.Info($"ActivateDictionary({id}) was requested");

            var result = false;

            if (ModelState.IsValid)
            {
                result = _dictionaryRepository.Activate(id, HttpContext.GetUserId());

                if (result)
                {
                    _loggerManager.Info($"ActivateDictionary({id}) successfully activaed");
                }
                else
                {
                    _loggerManager.Warn($"ActivateDictionary({id}) was unable to activate");
                }
            }
            else
            {
                _loggerManager.Warn($"ActivateDictionary({id}) was invalid");
            }

            return new JsonResult(new { result });
        }

        [HttpPost]
        public IActionResult DisactivateDictionary(int id)
        {
            _loggerManager.Info($"DisactivateDictionary({id}) was requested");

            var result = false;

            if (ModelState.IsValid)
            {
                result = _dictionaryRepository.Disactivate(id, HttpContext.GetUserId());

                if (result)
                {
                    _loggerManager.Info($"DisactivateDictionary({id}) successfully disactivated");
                }
                else
                {
                    _loggerManager.Warn($"DisactivateDictionary({id}) was unable to disactivate");
                }
            }
            else
            {
                _loggerManager.Warn($"DisactivateDictionary({id}) was invalid");
            }

            return new JsonResult(new { result });
        }

        [HttpGet]
        public IActionResult GetIntroductionList()
        {
            _loggerManager.Info($"GetIntroductionList was requested");

            return RedirectToAction("GetIntroduction", new { stepIndex = Steps.Predeparture });
        }

        [HttpGet]
        public IActionResult GetIntroduction(string stepIndex)
        {
            _loggerManager.Info($"GetIntroduction({stepIndex}) was requested");

            if (!_planRepository.GetStepList().Contains(stepIndex))
            {
                _loggerManager.Warn($"GetIntroduction({stepIndex}) : Step index is wrong");
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
        public IActionResult UploadIntroduction(IntroductionDTO introduction)
        {
            _loggerManager.Info($"UploadIntroduction was requested");

            var files = HttpContext.Request.Form.Files;

            if (files == null || files.Count != 1)
            {
                ModelState.AddModelError(string.Empty, "A file is invalid");

                _loggerManager.Warn($"UploadIntroduction : A file is invalid");

                return View("Introduction", introduction);
            }

            if (!_planRepository.GetStepList().Contains(introduction.Step))
            {
                _loggerManager.Warn($"UploadIntroduction : Step index is wrong");
                return BadRequest();
            }

            var uploadlimit = int.Parse(_settingRepository.Get(Settings.FileUploadLimit));

            if (!ValidationHelper.ValidateFileSize(files[0], uploadlimit))
            {
                ModelState.AddModelError(string.Empty, "A file exceed a limit");

                _loggerManager.Warn($"UploadIntroduction : A file exceed a limit");

                return View("Introduction", introduction);
            }

            var userId = HttpContext.GetUserId();

            var uploadRelPath = UploadHelper.Upload(files[0]);

            var fileDto = _fileRepository.CreateNewFile(Path.GetFileNameWithoutExtension(files[0].FileName), Path.GetExtension(files[0].FileName), uploadRelPath, userId);

            var result = false;

            if (fileDto != null)
            {
                _loggerManager.Info($"UploadIntroduction : A file was created");

                var oldVideo = _planRepository.GetIntroduction(introduction.Step)?.Video;
                result = _planRepository.UpdateIntroduction(introduction.Step, fileDto.Id, userId);

                if (result && oldVideo != null)
                {
                    bool oldFileDelete = false;
                    oldFileDelete = UploadHelper.Delete(oldVideo.Path);
                    oldFileDelete = _fileRepository.Delete(oldVideo.Id) && oldFileDelete;

                    if (oldFileDelete)
                    {
                        _loggerManager.Warn($"UploadIntroduction : An old file was deleted");
                    }
                    else
                    {
                        _loggerManager.Warn($"UploadIntroduction : An old file was not deleted");
                    }
                }
            }

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong due to a server issue");
                _loggerManager.Warn($"UploadIntroduction was unable to update");

                return View("Introduction", introduction);
            }
            else
            {
                _loggerManager.Info($"UploadIntroduction successfully updated");
            }

            return RedirectToAction("GetIntroduction", new { stepIndex = introduction.Step });
        }

        [HttpGet]
        public IActionResult GetBlockList(string stepIndex)
        {
            _loggerManager.Info($"GetBlockList({stepIndex}) was requested");

            if (string.IsNullOrWhiteSpace(stepIndex) || !_planRepository.GetStepList().Contains(stepIndex))
            {
                ViewData["Step"] = Steps.Predeparture;
            }
            else
            {
                ViewData["Step"] = stepIndex;
            }

            return View("BlockEdit");
        }

        [HttpGet]
        public IActionResult BlockEdit(int id)
        {
            _loggerManager.Info($"BlockEdit({id}) was requested");

            var block = _planRepository.GetBlock(id);

            if (block == null)
            {
                _loggerManager.Warn($"BlockEdit({id}): A block was not found");
                return BadRequest();
            }

            ViewData["Step"] = block.Step;

            var blockEdit = new BlockEditDTO
            {
                Description = block.Description,
                Id = block.Id,
                Instruction = block.Instruction,
                Title = block.Title,
                Step = block.Step
            };

            _loggerManager.Info($"BlockEdit({id}): A block was successfully found");

            return View("BlockEdit", blockEdit);
        }

        [HttpPost]
        public IActionResult BlockEdit(BlockEditDTO blockEdit)
        {
            _loggerManager.Info($"BlockEdit() was requested");

            ViewData["Step"] = blockEdit.Step;

            if (ModelState.IsValid)
            {
                var result = _planRepository.UpdateBlock(blockEdit, HttpContext.GetUserId());

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong due to a server issue");

                    _loggerManager.Warn($"BlockEdit() was unable to update a block");
                }
                else
                {
                    ViewData["SuccessMessage"] = "Block successfully updated";

                    _loggerManager.Warn($"BlockEdit() successfully updated");
                }
            }
            else
            {
                _loggerManager.Warn($"BlockEdit() was invalid");
            }

            return View("BlockEdit", blockEdit);
        }

        [HttpGet]
        public IActionResult GetSettingList()
        {
            _loggerManager.Info($"GetSettingList() was requested");

            return View("SettingList", _settingRepository.GetSettingList());
        }

        [HttpGet]
        public IActionResult EditSetting(string index)
        {
            _loggerManager.Info($"EditSetting({index}) GET was requested");

            var value = _settingRepository.Get(index);

            var settingDTO = new SettingDTO
            {
                Index = index,
                Value = value
            };

            return PartialView("~/Views/Manage/Partials/_EditSetting.cshtml", settingDTO);
        }

        [HttpPost]
        public IActionResult EditSetting(SettingDTO setting)
        {
            _loggerManager.Info($"EditSetting() POST was requested");

            var result = false;
            if (ModelState.IsValid)
            {
                result = _settingRepository.Set(setting.Index, setting.Value, HttpContext.GetUserId());

                if (result)
                {
                    _loggerManager.Info($"EditSetting() POST successfully updated");
                }
                else
                {
                    _loggerManager.Warn($"EditSetting() POST was unable to update");
                }
            }
            else
            {
                _loggerManager.Warn($"EditSetting() POST was invalid");
            }

            return PartialView("~/Views/Manage/Partials/_EditSetting.cshtml", setting);
        }

    }
}
