using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Web.Controllers
{
    public class WorksheetController : AbstractController
    {
        public readonly IPlanRepository _planRepository;

        public WorksheetController(IPlanRepository planRepository, ILoggerManager loggerManager) : base(loggerManager)
        {
            _planRepository = planRepository;
        }

        public IActionResult GetStep(string stepIndex,int planId)
        {
            var stepDTO = _planRepository.GetStep(stepIndex, planId);

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
            return RedirectToAction("GetStep", new { stepIndex = Steps.Predeparture, planId=id });
        }

        [HttpPost]
        public IActionResult GetPlanningTeam(int planId)
        {
            if(planId<=0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return PartialView("~/Views/Worksheet/Partials/_PlanningTeam.cshtml",Enumerable.Empty<UserPlanningMemberDTO>());
            }

            var planningTeam = _planRepository.GetPlanningTeam(planId);

            return PartialView("~/Views/Worksheet/Partials/_PlanningTeam.cshtml", planningTeam);
        }
    }
}