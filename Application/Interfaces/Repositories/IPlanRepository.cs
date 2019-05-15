using Application.DTOs;
using Core.Entities;
using System.Collections.Generic;

namespace Application.Interfaces.Repositories
{
    public interface IPlanRepository : IRepositoryBase<Plan>
    {
        void CreatePlan(Plan plan);

        PlanStepDTO GetStep(string stepIndex, int planId, bool isDefinitive, int userId);

        IEnumerable<PlanStepDTO> GetStepList();

        IEnumerable<PlanDTO> GetPlanList();

        bool AddUserToPlan(int userId, int planId, int positionId);

        bool RemoveUserFromPlan(int userId, int planId);

        IEnumerable<UserPlanningMemberDTO> GetPlanningTeam(int planId);

        bool SaveStep(PlanStepDTO planStep, bool isDefinitive, bool isSubmitted, int userId);

        IEnumerable<PlanDTO> GetPlanListForUser(int userId);
    }
}
