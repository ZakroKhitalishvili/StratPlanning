using Application.DTOs;
using Core.Entities;
using System.Collections.Generic;

namespace Application.Interfaces.Repositories
{
    /// <summary>
    /// Plan repository interface that extends IRepositoryBase
    /// </summary>
    public interface IPlanRepository : IRepositoryBase<Plan>
    {
        /// <summary>
        /// Creates a new plan
        /// </summary>
        /// <param name="plan">This should contain all requiring plan data</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns></returns>
        bool CreatePlan(PlanDTO plan, int userId);

        /// <summary>
        /// Deletes a plan
        /// </summary>
        /// <param name="planId">deleting plan's id</param>
        /// <returns></returns>
        bool DeletePlan(int planId);

        /// <summary>
        /// Gets a step for certain plan with block structure containing answers for invoker user
        /// </summary>
        /// <param name="stepIndex">Step identifying index</param>
        /// <param name="planId">Working plan's id</param>
        /// <param name="isDefinitive">Determines whether containing answers to be definitive</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns></returns>
        PlanStepDTO GetStep(string stepIndex, int planId, bool isDefinitive, int userId);

        /// <summary>
        /// Returns an ordered array of all step constants
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetStepList();

        /// <summary>
        /// Returns an ordered array of step constants filtered by action plan filter
        /// </summary>
        /// <param name="isActionPlan">Determines whether returning array should contain action plan's specific step</param>
        /// <returns></returns>
        IEnumerable<string> GetStepList(bool isActionPlan);

        /// <summary>
        /// Return an ordered array of action plan's specific steps
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetActionPlanSteps();

        /// <summary>
        /// Returns plan list
        /// </summary>
        /// <param name="searchText">Filters plans by Title and description</param>
        /// <param name="skipCount">Amount of plans to be skipped from beginning</param>
        /// <param name="takeCount">Amount of plans to be taken</param>
        /// <param name="totalCount">Total amount of plans should be assigned to this argument</param>
        /// <returns></returns>
        IEnumerable<PlanDTO> GetPlanList(string searchText, int skipCount, int takeCount, out int totalCount);

        /// <summary>
        /// Adds an user to a plannning team
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <param name="planId">Plan's Id</param>
        /// <param name="positionId">Id of position which was assigned to an user</param>
        /// <returns>Whether accomplished or not</returns>
        bool AddUserToPlan(int userId, int planId, int positionId);

        /// <summary>
        /// Adds an user to specific step planning team
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <param name="planId">Plan's Id</param>
        /// <param name="step">Identifying index for a step</param>
        /// <returns>Whether accomplished or no</returns>
        bool AddUserToPlanStep(int userId, int planId, string step);

        /// <summary>
        /// Removes user from a planning team
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <param name="planId">Plan's Id</param>
        /// <returns>Whether accomplished or no</returns>
        bool RemoveUserFromPlan(int userId, int planId);

        /// <summary>
        /// Completes a step
        /// </summary>
        /// <remarks>
        /// It does not complete if final definitive answers are not submitted
        /// </remarks>
        /// <param name="planid">Plan's id</param>
        /// <param name="stepIndex">Identifying index for a step</param>
        /// <returns>Whether accomplished or no</returns>
        bool CompleteStep(int planid, string stepIndex);

        /// <summary>
        /// Gets planning team for a plan
        /// </summary>
        /// <param name="planId">Plan's id</param>
        /// <returns></returns>
        IEnumerable<UserPlanningMemberDTO> GetPlanningTeam(int planId);

        /// <summary>
        /// Saves a step form
        /// </summary>
        /// <param name="planStep">Should contain answers</param>
        /// <param name="isDefinitive">Determines whether given answers to be saved a definitive answers</param>
        /// <param name="isSubmitted">submit or not answers for a step</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns></returns>
        bool SaveStep(PlanStepDTO planStep, bool isDefinitive, bool isSubmitted, int userId);

        /// <summary>
        /// Returns plan list where an user is joined in a planning team
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="searchText">Filters plans by Title and description</param>
        /// <param name="skipCount">Amount of plans to be skipped from beginning</param>
        /// <param name="takeCount">Amount of plans to be taken</param>
        /// <param name="totalCount">Total amount of plans should be assigned to this argument</param>
        /// <returns></returns>
        IEnumerable<PlanDTO> GetPlanListForUser(int userId, string searchText, int skipCount, int takeCount, out int totalCount);

        /// <summary>
        /// Gets file info list for a file typed question
        /// </summary>
        /// <param name="questionId">Questions' id</param>
        /// <param name="planId">Plan's id</param>
        /// <param name="userId">User's id</param>
        /// <param name="isDefinitive">If this is true, definitive files will be returned, otherwise specific files uploaded  a given user</param>
        /// <returns></returns>
        IList<FileDTO> GetFileAnswers(int questionId, int planId, int userId, bool isDefinitive);

        /// <summary>
        /// Gets approved definitive issue list
        /// </summary>
        /// <param name="planId">_Plan's id</param>
        /// <returns></returns>
        IList<IssueDTO> GetIssues(int planId);

        /// <summary>
        /// Gets approved definitive stakeholders
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="isInternal">If true returns internal stakeholders, otherwise externals</param>
        /// <returns></returns>
        IList<StakeholderDTO> GetDefinitiveStakehloders(int planId, bool isInternal);

        /// <summary>
        /// Gets approved definitive options for issues
        /// </summary>
        /// <param name="planId">Plan's id</param>
        /// <returns></returns>
        IList<IssueOptionAnsweDTO> GetDefinitiveIssueOptions(int planId);

        /// <summary>
        /// Gets all resources list
        /// </summary>
        /// <returns></returns>
        IList<ResourceDTO> GetResources();

        /// <summary>
        /// Gets approved definitive resources for a plan
        /// </summary>
        /// <param name="planId">Plan's id</param>
        /// <returns></returns>
        IList<ResourceDTO> GetResourcesByPlan(int planId);

        /// <summary>
        /// Determines whether an user can access a given step of a plan
        /// </summary>
        /// <param name="planId">Plan's id</param>
        /// <param name="stepIndex">Step index</param>
        /// <returns></returns>
        bool IsAvailableStep(int planId, string stepIndex);

        /// <summary>
        /// Gets a step which is active to be answered
        /// </summary>
        /// <param name="planId">Plan's id</param>
        /// <returns></returns>
        string GetWorkingStep(int planId);

        /// <summary>
        /// Determines whether an user is joined to a plan
        /// </summary>
        /// <param name="planId">Plan's id</param>
        /// <param name="userId">user's id</param>
        /// <returns></returns>
        bool IsUserInPlanningTeam(int planId, int userId);

        /// <summary>
        /// gets an introduction for a step
        /// </summary>
        /// <param name="stepIndex">Step index</param>
        /// <returns></returns>
        IntroductionDTO GetIntroduction(string stepIndex);

        /// <summary>
        /// gets introductions for all steps
        /// </summary>
        /// <returns></returns>
        IEnumerable<IntroductionDTO> GetIntroductions();

        /// <summary>
        /// Updates an introduction for a step with new video file's id
        /// </summary>
        /// <param name="stepIndex">Step index</param>
        /// <param name="VideoId">Video file's id</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool UpdateIntroduction(string stepIndex, int VideoId, int userId);

        /// <summary>
        /// gets all blocks from a step
        /// </summary>
        /// <param name="stepindex">Step index</param>
        /// <returns></returns>
        IEnumerable<BlockDTO> GetBlocks(string stepindex);

        /// <summary>
        /// Gets a block by id
        /// </summary>
        /// <param name="id">Block's id</param>
        /// <returns></returns>
        BlockDTO GetBlock(int id);

        /// <summary>
        /// Updates a block
        /// </summary>
        /// <param name="block">Block's updating data</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool UpdateBlock(BlockEditDTO block, int userId);
    }
}
