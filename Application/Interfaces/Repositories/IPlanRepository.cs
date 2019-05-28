﻿using Application.DTOs;
using Core.Entities;
using System.Collections.Generic;

namespace Application.Interfaces.Repositories
{
    public interface IPlanRepository : IRepositoryBase<Plan>
    {
        bool CreatePlan(PlanDTO plan, int userId);

        PlanStepDTO GetStep(string stepIndex, int planId, bool isDefinitive, int userId);

        IEnumerable<string> GetStepList();

        IEnumerable<PlanDTO> GetPlanList();

        bool AddUserToPlan(int userId, int planId, int positionId);

        bool AddUserToPlanPlanStep(int userId, int planId, string step);

        bool RemoveUserFromPlan(int userId, int planId);

        IEnumerable<UserPlanningMemberDTO> GetPlanningTeam(int planId);

        bool SaveStep(PlanStepDTO planStep, bool isDefinitive, bool isSubmitted, int userId);

        IEnumerable<PlanDTO> GetPlanListForUser(int userId);

        IList<FileDTO> GetFileAnswers(int questionId, int userId);

        IList<IssueDTO> GetIssues(int planId);

        IList<StakeholderDTO> GetDefinitiveStakehloders(int planId, bool isInternal);

        IList<ResourceDTO> GetResources();
    }
}
