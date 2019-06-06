using Application.DTOs;
using Core.Entities;
using System.Collections.Generic;

namespace Application.Interfaces.Repositories
{
    public interface IPlanRepository : IRepositoryBase<Plan>
    {
        bool CreatePlan(PlanDTO plan, int userId);

        bool DeletePlan(int planId);

        PlanStepDTO GetStep(string stepIndex, int planId, bool isDefinitive, int userId);

        IEnumerable<string> GetStepList();

        IEnumerable<string> GetStepList(bool isActionPlan);

        IEnumerable<string> GetActionPlanSteps();

        IEnumerable<PlanDTO> GetPlanList(int skipCount, int takeCount, out int totalCount);

        bool AddUserToPlan(int userId, int planId, int positionId);

        bool AddUserToPlanStep(int userId, int planId, string step);

        bool RemoveUserFromPlan(int userId, int planId);

        bool CompleteStep(int planid, string stepIndex);

        IEnumerable<UserPlanningMemberDTO> GetPlanningTeam(int planId);

        bool SaveStep(PlanStepDTO planStep, bool isDefinitive, bool isSubmitted, int userId);

        IEnumerable<PlanDTO> GetPlanListForUser(int userId, int skipCount, int takeCount, out int totalCount);

        IList<FileDTO> GetFileAnswers(int questionId, int planId, int userId, bool isDefinitive);

        IList<IssueDTO> GetIssues(int planId);

        IList<StakeholderDTO> GetDefinitiveStakehloders(int planId, bool isInternal);

        IList<IssueOptionAnsweDTO> GetDefinitiveIssueOptions(int planId);

        IList<ResourceDTO> GetResources();

        IList<ResourceDTO> GetResourcesByPlan(int planId);

        bool IsAvailableStep(int planId, string stepIndex);

        string GetWorkingStep(int planId);

        bool IsUserInPlanningTeam(int planId, int userId);
    }
}
