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

namespace Web.Controllers
{
    public class WorksheetController : AbstractController
    {
        public readonly IPlanRepository _planRepository;

        public readonly IFileRepository _fileRepository;

        public WorksheetController(IPlanRepository planRepository, ILoggerManager loggerManager, IFileRepository fileRepository) : base(loggerManager)
        {
            _planRepository = planRepository;
            _fileRepository = fileRepository;
        }

        public IActionResult GetStep(string stepIndex, int planId)
        {
            var userId = HttpContext.GetUserId();

            var isDefinitive = User.IsInRole(Roles.Admin);

            var stepDTO = _planRepository.GetStep(stepIndex, planId, isDefinitive, userId);

            ViewBag.Steps = _planRepository.GetStepList();

            return View("Step", stepDTO);
        }

        public IActionResult Index()
        {
            return RedirectToAction("GetPlanList");
        }

        public IActionResult GetPlanList()
        {
            if (User.IsInRole(Roles.Admin))
            {
                var planList = _planRepository.GetPlanList();

                return View("PlanList", planList);
            }
            else
            {
                var planList = _planRepository.GetPlanListForUser(HttpContext.GetUserId());

                return View("PlanList", planList);
            }
        }

        public IActionResult GetPlan(int id)
        {
            return RedirectToAction("GetStep", new { stepIndex = Steps.Predeparture, planId = id });
        }

        [HttpPost]
        public IActionResult GetPlanningTeam(int planId)
        {
            if (planId <= 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return PartialView("~/Views/Worksheet/Partials/_PlanningTeam.cshtml", Enumerable.Empty<UserPlanningMemberDTO>());
            }

            var planningTeam = _planRepository.GetPlanningTeam(planId);

            return PartialView("~/Views/Worksheet/Partials/_PlanningTeam.cshtml", planningTeam);
        }

        [HttpPost]
        public IActionResult SaveStep(PlanStepDTO planStep)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
            var isDefinitive = User.IsInRole(Roles.Admin);

            bool result = false;

            if (ModelState.IsValid)
            {
                result = _planRepository.SaveStep(planStep, isDefinitive, planStep.IsSubmitted, userId: HttpContext.GetUserId());

                if (result)
                {
                    ModelState.Clear();
                    HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                }
            }

            var newPlanStep = _planRepository.GetStep(planStep.Step, planStep.PlanId, isDefinitive, HttpContext.GetUserId());

            if (!result)
            {
                newPlanStep.FilledAnswers = planStep.AnswerGroups;
            }

            return PartialView("~/Views/Worksheet/Partials/_StepForm.cshtml", newPlanStep);
        }

        [HttpGet]
        public IActionResult GetAnswerFiles(int questionId)
        {
            var userId = HttpContext.GetUserId();

            var files = _planRepository.GetFileAnswers(questionId, userId);

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
            var files = HttpContext.Request.Form.Files;

            if (files == null || files.Count == 0)
            {
                return BadRequest();
            }

            var result = new List<FileDTO>();

            var userId = HttpContext.GetUserId();

            foreach (var file in files)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads",
                                       file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var fileDto = _fileRepository.CreateNewFile(Path.GetFileNameWithoutExtension(path), Path.GetExtension(path), $"/Uploads/{file.FileName}", userId);

                if (fileDto != null) result.Add(fileDto);

            }

            return Ok(result.Select(x => new
            {
                id = x.Id,
                name = x.Name + x.Ext,
                url = x.Path
            }).ToArray());
        }

        [HttpPost]
        public IActionResult CompleteStepTask(int stepTaskId)
        {
            if (stepTaskId <= 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult RefreshStepForm(PlanStepDTO planStep, bool? keepFilled)
        {
            var isDefinitive = User.IsInRole(Roles.Admin);

            var newPlanStep = _planRepository.GetStep(planStep.Step, planStep.PlanId, isDefinitive, HttpContext.GetUserId());

            if (keepFilled != null && keepFilled.Value)
            {
                newPlanStep.FilledAnswers = planStep.AnswerGroups;
                foreach (var stepTaskAnswer in planStep.StepTaskAnswers.Answer.StepTaskAnswers.Where(x => x.Id == 0))
                {
                    newPlanStep.StepTaskAnswers.Answer.StepTaskAnswers?.Add(stepTaskAnswer);
                }

            }

            return PartialView("~/Views/Worksheet/Partials/_StepForm.cshtml", newPlanStep);
        }

    }
}