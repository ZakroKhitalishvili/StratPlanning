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

            TryValidateModel(planStep.AnswerGroups);

            if (ModelState.IsValid)
            {
                result = _planRepository.SaveStep(planStep, isDefinitive, planStep.IsSubmitted, userId: HttpContext.GetUserId());

                if (result)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            var newPlanStep = _planRepository.GetStep(planStep.Step, planStep.PlanId, isDefinitive, HttpContext.GetUserId());

            if (!result)
            {
                newPlanStep.FilledAnswers = planStep.AnswerGroups;
            }

            return PartialView("~/Views/Worksheet/Partials/_StepForm.cshtml", newPlanStep);
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            var files = HttpContext.Request.Form.Files;

            if (files == null || files.Count == 0)
            {
                return BadRequest();
            }
            int id = 0;
            foreach (var file in files)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads",
                                       file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var fileEntity = new Core.Entities.File
                {

                };

                //_fileRepository.Create(fileEntity);
                //id= fileEntity.Id;

            }

            return Ok(new { fileId = id });
        }

    }
}