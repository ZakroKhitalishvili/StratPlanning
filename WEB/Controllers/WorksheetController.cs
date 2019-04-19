using Application.Interfaces;
using Application.Interfaces.Repositories;
using Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class WorksheetController : AbstractController
    {
        public readonly IPlanRepository _planRepository;

        public WorksheetController(IPlanRepository planRepository, ILoggerManager loggerManager) : base(loggerManager)
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
            return RedirectToAction("GetPlanList");
        }

        public IActionResult GetPlanList()
        {
            var planList = _planRepository.GetPlanList();

            return View("PlanList", planList);
        }

        public IActionResult GetPlan(int id)
        {
            return RedirectToAction("GetStep", new { stepIndex = Steps.Predeparture });
        }
    }
}