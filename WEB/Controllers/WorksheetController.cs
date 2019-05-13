using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Extensions;

namespace Web.Controllers
{
    public class WorksheetController : AbstractController
    {
        public readonly IPlanRepository _planRepository;

        public WorksheetController(IPlanRepository planRepository, ILoggerManager loggerManager) : base(loggerManager)
        {
            _planRepository = planRepository;
        }

        public IActionResult GetStep(string stepIndex, int planId)
        {
            var userId = HttpContext.GetUserId();

            var stepDTO = _planRepository.GetStep(stepIndex, planId, userId);

            ViewBag.Steps = _planRepository.GetStepList();

            return View("Step", stepDTO);
        }

        public IActionResult Index()
        {
            return RedirectToAction("GetPlanList");
        }

        public IActionResult GetPlanList()
        {
            var planList = _planRepository.GetPlanList();

            return View("PlanList", planList);
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

            bool result = false;

            TryValidateModel(planStep.AnswerGroups);

            if (ModelState.IsValid)
            {
                //var isDefinitive = User.IsInRole(Roles.Admin);

                var isDefinitive = false;

                result = _planRepository.SaveStep(planStep, isDefinitive, isSubmitted: false, userId: HttpContext.GetUserId());

                if (result)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                }
            }

            var newPlanStep = _planRepository.GetStep(planStep.Step, planStep.PlanId, HttpContext.GetUserId());

            if (!result)
            {
                newPlanStep.FilledAnswers = planStep.AnswerGroups;
            }

            return PartialView("~/Views/Worksheet/Partials/_StepForm.cshtml", newPlanStep);
        }


    }
}