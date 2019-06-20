using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Web.Extensions;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using X.PagedList;
using Web.Helpers;

namespace Web.Controllers
{
    public class WorksheetController : AbstractController
    {
        public readonly IPlanRepository _planRepository;

        public readonly IFileRepository _fileRepository;

        public readonly IHostingEnvironment _hostingEnvironment;

        public readonly ISettingRepository _settingRepository;

        public WorksheetController(IPlanRepository planRepository,
            ILoggerManager loggerManager,
            IFileRepository fileRepository,
            IHostingEnvironment hostingEnvironment,
            ISettingRepository settingRepository) : base(loggerManager)
        {
            _planRepository = planRepository;
            _fileRepository = fileRepository;
            _hostingEnvironment = hostingEnvironment;
            _settingRepository = settingRepository;
        }

        public IActionResult GetStep(string stepIndex, int planId)
        {
            _loggerManager.Info($"GetStep({stepIndex},{planId}) is requested");

            if (string.IsNullOrEmpty(stepIndex) || !_planRepository.GetStepList().Contains(stepIndex))
            {
                _loggerManager.Warn($"GetStep({stepIndex},{planId}): Step Index is invalid");
                return BadRequest();
            }

            if (!_planRepository.FindByCondition(x => x.Id == planId).Any())
            {
                _loggerManager.Warn($"GetStep({stepIndex},{planId}): A plan with a such Id does not exist");
                return BadRequest();
            }

            var userId = HttpContext.GetUserId();

            var isDefinitive = User.IsInRole(Roles.Admin);

            if (!isDefinitive && !_planRepository.IsUserInPlanningTeam(planId, userId))
            {
                _loggerManager.Warn($"GetStep({stepIndex},{planId}): an users is not authorized to access");
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            if (!_planRepository.IsAvailableStep(planId, stepIndex))
            {
                _loggerManager.Warn($"GetStep({stepIndex},{planId}): The step is not avalaible");
                return BadRequest();
            }

            var stepDTO = _planRepository.GetStep(stepIndex, planId, isDefinitive, userId);

            if (stepDTO == null)
            {
                _loggerManager.Error($"GetStep({stepIndex},{planId}): Internal error ");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            _loggerManager.Info($"GetStep({stepIndex},{planId}): Successfully returned step");
            return View("Step", stepDTO);
        }

        public IActionResult Index()
        {
            _loggerManager.Info($"Worksheet.Index() Redirected to GetPlanList()");
            return RedirectToAction("GetPlanList");
        }

        public IActionResult GetPlanList(int? page)
        {
            _loggerManager.Info($"GetPlanList({page}) was requested");

            var parseResult = int.TryParse(_settingRepository.Get(Settings.PageSize), out int pageSize);

            if (!parseResult || pageSize <= 0)
            {
                pageSize = 5;
            }

            var skipCount = ((page ?? 1) - 1) * pageSize;
            var takeCount = pageSize;

            if (User.IsInRole(Roles.Admin))
            {
                var planList = _planRepository.GetPlanList(skipCount, takeCount, out int totalCount);

                var pagedList = new StaticPagedList<PlanDTO>(planList, page ?? 1, pageSize, totalCount);

                _loggerManager.Info($"GetPlanList({page}) returned a list for admin view");

                return View("PlanList", pagedList);
            }
            else
            {
                var planList = _planRepository.GetPlanListForUser(HttpContext.GetUserId(), skipCount, takeCount, out int totalCount);

                var pagedList = new StaticPagedList<PlanDTO>(planList, page ?? 1, pageSize, totalCount);

                _loggerManager.Info($"GetPlanList({page}) returned a list for employee view");

                return View("PlanList", pagedList);
            }
        }

        public IActionResult GetPlan(int id)
        {
            _loggerManager.Info($"GetPlan({id}) was requested and redirected to GetStep() for a working step");
            return RedirectToAction("GetStep", new { stepIndex = _planRepository.GetWorkingStep(id), planId = id });
        }

        [HttpPost]
        public IActionResult GetPlanningTeam(int planId)
        {
            _loggerManager.Info($"GetPlanningTeam({planId}) was requested");
            if (planId <= 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;

                _loggerManager.Warn($"GetPlanningTeam({planId}) was bad request");

                return PartialView("~/Views/Worksheet/Partials/_PlanningTeam.cshtml", Enumerable.Empty<UserPlanningMemberDTO>());
            }

            var planningTeam = _planRepository.GetPlanningTeam(planId);

            _loggerManager.Info($"GetPlanningTeam({planId}) successfully returned a result");

            return PartialView("~/Views/Worksheet/Partials/_PlanningTeam.cshtml", planningTeam);
        }

        [HttpPost]
        public IActionResult SaveStep(PlanStepDTO planStep)
        {
            _loggerManager.Info($"SaveStep() was requested for a step ({planStep.Step}) of a plan({planStep.PlanId})");

            HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
            var isDefinitive = User.IsInRole(Roles.Admin);

            bool result = false;

            if (ModelState.IsValid)
            {

                if (_planRepository.GetWorkingStep(planStep.PlanId) != planStep.Step)
                {
                    _loggerManager.Warn($"SaveStep's request appeared bad for step ({planStep.Step}) of a plan({planStep.PlanId})");
                    return BadRequest();
                }

                result = _planRepository.SaveStep(planStep, isDefinitive, planStep.IsSubmitted, userId: HttpContext.GetUserId());

                if (result)
                {
                    ModelState.Clear();

                    _loggerManager.Info($"Step({planStep.Step}) was successfully saved of a plan({planStep.PlanId})");
                    HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                }
            }

            var newPlanStep = _planRepository.GetStep(planStep.Step, planStep.PlanId, isDefinitive, HttpContext.GetUserId());

            if (!result)
            {
                _loggerManager.Warn($"SaveStep() request was invalid for a step ({planStep.Step}) of a plan({planStep.PlanId})");
                newPlanStep.FilledAnswers = planStep.AnswerGroups;
            }

            return PartialView("~/Views/Worksheet/Partials/_StepForm.cshtml", newPlanStep);
        }

        [HttpGet]
        public IActionResult GetAnswerFiles(int questionId, int planId)
        {
            _loggerManager.Info($"GetAnswerFiles({questionId},{planId}) was requested");

            var isDefinitive = User.IsInRole(Roles.Admin);

            var userId = HttpContext.GetUserId();

            var files = _planRepository.GetFileAnswers(questionId, planId, userId, isDefinitive);

            _loggerManager.Info($"GetAnswerFiles({questionId},{planId}) successfully returned  a result");

            return Ok(files.Select(x => new
            {
                id = x.Id,
                name = x.Name + x.Ext,
                url = x.Path
            }).ToArray());
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            _loggerManager.Info($"UploadFile() was requested");

            var files = HttpContext.Request.Form.Files;

            if (files == null || files.Count == 0)
            {
                _loggerManager.Warn($"UploadFile() request is a bad request");
                return BadRequest();
            }

            var result = new List<FileDTO>();

            var userId = HttpContext.GetUserId();

            var validExtensions = _settingRepository.Get(Settings.FileExtensionsForStep)?.Split(',');

            var uploadlimit = int.Parse(_settingRepository.Get(Settings.FileUploadLimit));

            foreach (var file in files)
            {
                if (ValidationHelper.ValidateFileExtension(file, validExtensions) && ValidationHelper.ValidateFileSize(file, uploadlimit))
                {
                    var uploadRelPath = UploadHelper.Upload(files[0]);

                    var fileDto = _fileRepository.CreateNewFile(Path.GetFileNameWithoutExtension(file.FileName), Path.GetExtension(file.FileName), uploadRelPath, userId);

                    if (fileDto != null) result.Add(fileDto);
                }
                else
                {
                    _loggerManager.Warn($"UploadFile() : {file.FileName} is not valid due to wrong extension or size");

                    Response.StatusCode = StatusCodes.Status400BadRequest;

                    return new JsonResult(new { message = $"{file.FileName} is not valid due to wrong extension or size" });
                }
            }

            _loggerManager.Info($"UploadFile() successfuly uploaded file(s) and returned a result");

            return Ok(result.Select(x => new
            {
                id = x.Id,
                name = x.Name + x.Ext,
                url = x.Path
            }).ToArray());
        }

        public IActionResult DeleteFile(int id)
        {
            _loggerManager.Info($"DeleteFile({id}) was requested");

            var file = _fileRepository.GetFile(id);

            if (file == null)
            {
                _loggerManager.Warn($"DeleteFile({id}) request is a bad request");
                return BadRequest();
            }

            var result = _fileRepository.Delete(id);

            if (result)
            {
                _loggerManager.Info($"DeleteFile({id}) deleted a file info from database");

                if (UploadHelper.Delete(file.Path))
                {
                    _loggerManager.Info($"DeleteFile({id}) deleted a file from a directory");
                }
                else
                {
                    _loggerManager.Info($"DeleteFile({id}) was unable to delete a file from a directory");
                }
            }

            return Ok(new { result });
        }

        [HttpPost]
        public IActionResult CompleteStepTask(int planId, string stepIndex)
        {
            _loggerManager.Info($"CompleteStepTask({planId},{stepIndex}) is requested");

            if (planId <= 0 || string.IsNullOrEmpty(stepIndex))
            {
                _loggerManager.Warn($"CompleteStepTask({planId},{stepIndex}) is bad request");
                return BadRequest();
            }

            var result = _planRepository.CompleteStep(planId, stepIndex);

            if (result)
            {
                _loggerManager.Info($"CompleteStepTask({planId},{stepIndex}) successfully completed task");

                return Ok();
            }
            else
            {
                _loggerManager.Warn($"CompleteStepTask({planId},{stepIndex}) was unable to complete task");

                return new StatusCodeResult(StatusCodes.Status202Accepted);
            }

        }

        [HttpPost]
        public IActionResult RefreshStepForm(PlanStepDTO planStep, bool? keepFilled)
        {
            _loggerManager.Info($"RefreshStepForm wa requsted for a step({planStep.Step}) of a plan({planStep.PlanId}) with option: keepFilled = {keepFilled ?? false}");

            var isDefinitive = User.IsInRole(Roles.Admin);

            var newPlanStep = _planRepository.GetStep(planStep.Step, planStep.PlanId, isDefinitive, HttpContext.GetUserId());

            if (keepFilled != null && keepFilled.Value)
            {
                newPlanStep.FilledAnswers = planStep.AnswerGroups;
                if (planStep.StepTaskAnswers?.Answer?.StepTaskAnswers != null)
                {
                    foreach (var stepTaskAnswer in planStep.StepTaskAnswers.Answer.StepTaskAnswers.Where(x => x.Id == 0))
                    {
                        newPlanStep.StepTaskAnswers.Answer.StepTaskAnswers?.Add(stepTaskAnswer);
                    }
                }
            }

            _loggerManager.Info($"RefreshStepForm successfully returned form for a step({planStep.Step}) of a plan({planStep.PlanId})");

            return PartialView("~/Views/Worksheet/Partials/_StepForm.cshtml", newPlanStep);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult CreatePlan(PlanDTO plan)
        {
            _loggerManager.Info($"CreatePlan is requested");

            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                var result = _planRepository.CreatePlan(plan, HttpContext.GetUserId());

                if (result)
                {
                    _loggerManager.Info($"CreatePlan successfully created a plan");
                    Response.StatusCode = StatusCodes.Status201Created;
                }
                else
                {
                    _loggerManager.Warn($"CreatePlan was unable to create a plan");
                    ModelState.AddModelError(string.Empty, "Something went wrong during creating plan");
                }
            }
            else
            {
                _loggerManager.Info($"CreatePlan request is invalid");
            }

            return PartialView("~/Views/Worksheet/Partials/_NewPlan.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult DeletePlan(int planId)
        {
            _loggerManager.Info($"DeletePlan({planId}) is requested");

            if (planId <= 0)
            {
                _loggerManager.Info($"DeletePlan({planId}) is a bad request");
                return BadRequest();
            }

            var result = _planRepository.DeletePlan(planId);

            if (result)
            {
                _loggerManager.Info($"DeletePlan({planId}) successfully deleted a plan");
            }
            else
            {
                _loggerManager.Info($"DeletePlan({planId}) is unable to delete a plan");
            }

            return new JsonResult(new { result });

        }
    }
}