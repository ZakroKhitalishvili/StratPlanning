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

namespace Web.Controllers
{
    public class WorksheetController : AbstractController
    {
        public readonly IPlanRepository _planRepository;

        public readonly IFileRepository _fileRepository;

        public readonly IHostingEnvironment _hostingEnvironment;

        public WorksheetController(IPlanRepository planRepository, ILoggerManager loggerManager, IFileRepository fileRepository, IHostingEnvironment hostingEnvironment) : base(loggerManager)
        {
            _planRepository = planRepository;
            _fileRepository = fileRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult GetStep(string stepIndex, int planId)
        {
            if(string.IsNullOrEmpty(stepIndex) || planId<=0)
            {
                return BadRequest();
            }

            if(!_planRepository.IsAvailableStep(planId,stepIndex))
            {
                return BadRequest();
            }

            var userId = HttpContext.GetUserId();

            var isDefinitive = User.IsInRole(Roles.Admin);

            var stepDTO = _planRepository.GetStep(stepIndex, planId, isDefinitive, userId);

            if(stepDTO==null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return View("Step", stepDTO);
        }

        public IActionResult Index()
        {
            return RedirectToAction("GetPlanList");
        }

        public IActionResult GetPlanList(int? page)
        {
            var pageSize = 5;
            var skipCount = ((page ?? 1 )- 1) * pageSize;
            var takeCount = pageSize;

            if (User.IsInRole(Roles.Admin))
            {
                var planList = _planRepository.GetPlanList(skipCount, takeCount, out int totalCount);

                var pagedList = new StaticPagedList<PlanDTO>(planList, page ?? 1, pageSize, totalCount);

                return View("PlanList", pagedList);
            }
            else
            {
                var planList = _planRepository.GetPlanListForUser(HttpContext.GetUserId(), skipCount, takeCount, out int totalCount);

                var pagedList = new StaticPagedList<PlanDTO>(planList, page ?? 1, pageSize, totalCount);

                return View("PlanList", pagedList);
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


            if (!_planRepository.IsAvailableStep(planStep.PlanId, planStep.Step))
            {
                return BadRequest();
            }

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
        public IActionResult GetAnswerFiles(int questionId, int planId)
        {
            var isDefinitive = User.IsInRole(Roles.Admin);

            var userId = HttpContext.GetUserId();

            var files = _planRepository.GetFileAnswers(questionId, planId, userId, isDefinitive);

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
                //var path = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads",
                //                       file.FileName);

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
        public IActionResult CompleteStepTask(int planId, string stepIndex)
        {
            if (planId <= 0 || string.IsNullOrEmpty(stepIndex))
            {
                return BadRequest();
            }

            var result = _planRepository.CompleteStep(planId, stepIndex);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        public IActionResult RefreshStepForm(PlanStepDTO planStep, bool? keepFilled)
        {
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

            return PartialView("~/Views/Worksheet/Partials/_StepForm.cshtml", newPlanStep);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult CreatePlan(PlanDTO plan)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

            if (ModelState.IsValid)
            {
                var result = _planRepository.CreatePlan(plan, HttpContext.GetUserId());

                if (result)
                {
                    Response.StatusCode = StatusCodes.Status201Created;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong during creating plan");
                }
            }

            return PartialView("~/Views/Worksheet/Partials/_NewPlan.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult DeletePlan(int planId)
        {
            Response.StatusCode = StatusCodes.Status202Accepted;

            if (planId <= 0)
            {
                return BadRequest();
            }

            var result = _planRepository.DeletePlan(planId);

            return new JsonResult(new { result });

        }
    }
}