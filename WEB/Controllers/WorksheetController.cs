using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class WorksheetController : Controller
    {
        public readonly IPlanRepository _planRepository;

        public WorksheetController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public IActionResult GetStep(string stepIndex)
        {
            var stepDTO = _planRepository.GetStep(stepIndex);

            ViewBag.Steps = _planRepository.GetStepList();

            return View("Step", stepDTO);
        }

        public IActionResult Index()
        {
            return RedirectToAction("GetStep", new { stepIndex = Steps.Predeparture });
        }

        public IActionResult GetPlanList()
        {
            var planList = _planRepository.GetPlanList();

            return View("PlanList",planList);
        }
    }
}