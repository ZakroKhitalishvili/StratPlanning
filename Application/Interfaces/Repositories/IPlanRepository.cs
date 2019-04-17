using Application.DTOs;
using Core.Entities;
using System.Collections.Generic;

namespace Application.Interfaces.Repositories
{
    public interface IPlanRepository:IRepositoryBase<Plan>
    {
        void CreatePlan(Plan plan);

        PlanStepDTO GetStep(string stepIndex);

        IEnumerable<PlanStepDTO> GetStepList();

        IEnumerable<PlanDTO> GetPlanList();
    }
}
