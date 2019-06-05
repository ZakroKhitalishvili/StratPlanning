using Core.Context;
using Core.Entities;
using System;
using Application.Interfaces.Repositories;
using Application.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Core.Constants;
using AutoMapper;
using System.Linq.Expressions;

namespace Application.Repositories
{
    public class PlanRepository : RepositoryBase<Plan>, IPlanRepository
    {
        public PlanRepository(PlanningDbContext context) : base(context)
        { }

        public bool AddUserToPlan(int userId, int planId, int positionId)
        {
            if (Context.UsersToPlans.Where(x => x.UserId == userId && x.PlanId == planId && x.Step == null).Any())
            {
                return false;
            }

            Context.UsersToPlans.Add(new UserToPlan { UserId = userId, PlanId = planId, PositionId = positionId });

            try
            {
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool AddUserToPlanStep(int userId, int planId, string step)
        {
            if (Context.UsersToPlans.Where(x => x.UserId == userId && x.PlanId == planId && x.Step == step).Any())
            {
                return false;
            }

            Context.UsersToPlans.Add(new UserToPlan { UserId = userId, PlanId = planId, Step = step });

            try
            {
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool CreatePlan(PlanDTO plan, int userId)
        {
            var newPlan = new Plan
            {
                Name = plan.Name,
                Description = plan.Description,
                IsCompleted = false,
                StartDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId
            };

            try
            {
                Context.Plans.Add(newPlan);
                Context.SaveChanges();

                var steps = this.GetStepList();
                foreach (var step in steps)
                {
                    GetOrCreateStepTask(newPlan.Id, step, userId);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool DeletePlan(int planId)
        {
            var plan = Get(planId);

            if (plan == null)
            {
                return false;
            }

            var stepTasks = Context.StepTasks.Where(x => x.PlanId == planId);

            try
            {
                Context.StepTasks.RemoveRange(stepTasks);

                var userStepResults = Context.UserStepResults.Include(x => x.UserToPlan)
                    .Where(x => (x.UserToPlan != null && x.UserToPlan.PlanId == planId) || x.PlanId.Value == planId);

                Context.UserStepResults.RemoveRange(userStepResults);

                Delete(plan);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool CompleteStep(int planId, string stepIndex)
        {
            if (IsAvailableStep(planId, stepIndex))
            {
                var stepTask = GetStepTask(planId, stepIndex);

                if (!stepTask.IsCompleted)
                {
                    stepTask.IsCompleted = true;
                    Context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<PlanDTO> GetPlanList(int skipCount, int takeCount,out int totalCount)
        {
            totalCount = Context.Plans.Count();

            return Context.Plans.OrderByDescending(x => x.CreatedAt).Skip(skipCount).Take(takeCount).Select(p => Mapper.Map<PlanDTO>(p));
        }

        public IEnumerable<UserPlanningMemberDTO> GetPlanningTeam(int planId)
        {
            return Context.UsersToPlans
                .Where(x => x.PlanId == planId && x.Step == null).Include(x => x.User).Include(x => x.Position)
                .AsEnumerable()
                .Select(x => new UserPlanningMemberDTO { Id = x.User.Id, UserToPlanId = x.Id, FullName = $"{x.User.FirstName} {x.User.LastName}", Position = x.Position?.Title })
                .ToList();
        }

        public PlanStepDTO GetStep(string stepIndex, int planId, bool isDefinitive, int userId)
        {
            //if(!IsAvailableStep(planId,stepIndex))
            //{
            //    return null;
            //}

            var planStep = GetPlanStep(planId, stepIndex, isDefinitive);

            UserStepResult currentUserStepResult = GetOrCreateUserStepResult(planId, stepIndex, isDefinitive, userId);

            IList<UserStepResult> otherUserStepResults = GetPlanStepResults(planId, stepIndex).Where(x => x.IsSubmitted).ToList();

            if (!isDefinitive)
            {
                var stepResult = otherUserStepResults.Where(x => x.UserToPlanId != null && x.UserToPlan.User.Id == userId).SingleOrDefault();
                otherUserStepResults.Remove(stepResult);

                var adminStepResult = GetSubmittedDefinitiveStepResult(planId, stepIndex);

                if (adminStepResult != null && adminStepResult.IsSubmitted)
                {
                    otherUserStepResults.Add(adminStepResult);
                }
            }

            planStep.IsSubmitted = currentUserStepResult.IsSubmitted;

            FillWithAnswers(planStep, currentUserStepResult, otherUserStepResults);

            return planStep;
        }

        public IEnumerable<string> GetStepList()
        {
            var members = typeof(Steps).GetMembers().Where(x => x.MemberType == System.Reflection.MemberTypes.Field);
            return members.Select(member => member.Name);
        }

        public bool RemoveUserFromPlan(int userId, int planId)
        {
            var userToPlan = Context.UsersToPlans.Where(x => x.UserId == userId && x.PlanId == planId).FirstOrDefault();

            if (userToPlan == null)
            {
                return false;
            }

            Context.UsersToPlans.Remove(userToPlan);

            try
            {
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SaveStep(PlanStepDTO planStep, bool isDefinitive, bool isSubmitted, int userId)
        {
            //if (!IsAvailableStep(planStep.PlanId, planStep.Step))
            //{
            //    return false;
            //}

            if (!isDefinitive)
            {
                var userStepResult = GetOrCreateUserStepResult(planStep.PlanId, planStep.Step, false, userId);

                if (userStepResult.IsSubmitted)
                {
                    return false;
                }
                if (isSubmitted)
                {
                    userStepResult.IsSubmitted = true;
                }

                SaveAnswers(planStep.AnswerGroups, userStepResult);

                if (planStep.Step == Steps.Predeparture)
                {
                    SaveStepTaskAnswers(planStep.StepTaskAnswers, userStepResult);
                }
            }
            else
            {
                // definitive answer
                // this code determines which stepResult should be handled (submitted one or final - only admin viewed one)
                var finalDefinitiveStepResult = GetFinalDefinitiveStepResult(planStep.PlanId, planStep.Step);
                if (finalDefinitiveStepResult == null)
                {
                    finalDefinitiveStepResult = CreateUserStepResult(planStep.PlanId, planStep.Step, isDefinitive, userId);
                    if (isSubmitted)
                    {
                        finalDefinitiveStepResult.IsSubmitted = true;
                    }
                }
                else
                {
                    var submittedDefinitiveResult = GetSubmittedDefinitiveStepResult(planStep.PlanId, planStep.Step);

                    if (isSubmitted)
                    {
                        if (!finalDefinitiveStepResult.IsSubmitted)
                        {
                            finalDefinitiveStepResult.IsSubmitted = true;

                            if (submittedDefinitiveResult != null)
                            {
                                submittedDefinitiveResult.IsSubmitted = false;
                            }
                        }
                    }
                    else
                    {
                        if (submittedDefinitiveResult != null)
                        {
                            if (submittedDefinitiveResult.Id == finalDefinitiveStepResult.Id)
                            {
                                var newFinalDefinitiveStepResult = CreateUserStepResult(planStep.PlanId, planStep.Step, isDefinitive, userId);
                                submittedDefinitiveResult.IsFinal = false;
                                finalDefinitiveStepResult = newFinalDefinitiveStepResult;
                            }
                        }
                    }
                }

                SaveAnswers(planStep.AnswerGroups, finalDefinitiveStepResult);

                //delete old stepResults that are neither submitted nor final
                var otherDefinitiveResult = Context.UserStepResults.Where(x => x.PlanId == planStep.PlanId && x.IsFinal.HasValue && !x.IsFinal.Value && !x.IsSubmitted && x.IsDefinitive && x.Step == planStep.Step).SingleOrDefault();

                if (otherDefinitiveResult != null)
                {
                    DeleteUserStepResult(otherDefinitiveResult.Id);
                }

                if (planStep.Step == Steps.Predeparture)
                {
                    SaveStepTaskAnswers(planStep.StepTaskAnswers, finalDefinitiveStepResult);
                }

            }

            if (planStep.Step == Steps.Predeparture)
            {
                SaveStepTasks(planStep.StepTasks, userId);
            }

            Context.SaveChanges();
            return true;
        }

        public IEnumerable<PlanDTO> GetPlanListForUser(int userId, int skipCount, int takeCount, out int totalCount)
        {
            totalCount = Context.UsersToPlans
                .Where(x => x.UserId == userId).Count();

            return Context.UsersToPlans
                .Where(x => x.UserId == userId)
                .Include(x => x.Plan).Include(x => x.UserStepResults).AsEnumerable()
                .OrderByDescending(x =>
                {
                    if (x.UserStepResults.Any()) return x.UserStepResults.Max(y => y.UpdatedAt);

                    return DateTime.MinValue;
                })
                .Skip(skipCount).Take(takeCount)
                .Select(x => Mapper.Map<PlanDTO>(x.Plan)).ToList();
        }

        public IList<FileDTO> GetFileAnswers(int questionId, int planId, int userId, bool isDefinitive)
        {
            return Context.UserStepResults
                .Include(x => x.UserToPlan)
                .Include(x => x.FileAnswers)
                .Where(x => (isDefinitive && x.PlanId == planId && x.IsDefinitive) || (x.UserToPlan.UserId == userId && x.UserToPlan.PlanId == planId))
                .SelectMany(x => x.FileAnswers)
                .Where(x => x.QuestionId == questionId)
                .Select(x => Mapper.Map<FileDTO>(x.File)).ToList();
        }

        public IList<IssueDTO> GetIssues(int planId)
        {
            var swotDefinitiveAnswer = GetSubmittedDefinitiveStepResult(planId, Steps.SWOT);
            return (swotDefinitiveAnswer?.SWOTAnswers.Where(x => x.IsIssue)
                .Select(x => new IssueDTO { Id = x.Id, Name = x.Name })
                ?? Enumerable.Empty<IssueDTO>())
                .ToList();
        }

        public IList<StakeholderDTO> GetDefinitiveStakehloders(int planId, bool isInternal)
        {
            var adminStepResult = GetSubmittedDefinitiveStepResult(planId, Steps.StakeholdersIdentify);

            if (adminStepResult == null)
            {
                return new List<StakeholderDTO>();
            }
            return adminStepResult.StakeholderAnswers.Where(x => x.IsInternal == isInternal)
                .Select(x => new StakeholderDTO
                {
                    Id = x.Id,
                    Name = x.FirstName + " " + x.LastName
                }).ToList();
        }

        public IList<IssueOptionAnsweDTO> GetDefinitiveIssueOptions(int planId)
        {
            var adminStepResult = GetSubmittedDefinitiveStepResult(planId, Steps.ActionPlanKeyQuestions);

            if (adminStepResult == null)
            {
                return new List<IssueOptionAnsweDTO>();
            }

            return adminStepResult.IssueOptionAnswers.Where(x => x.IsBestOption)
                    .Select(x => new IssueOptionAnsweDTO
                    {
                        Id = x.Id,
                        Option = x.Option,
                        Actors = x.Actors,
                        IssueId = x.IssueId,
                        Resources = String.Join(',', x.IssueOptionAnswersToResources.Select(y => y.Resource.Title).ToList()),
                        IssueName = x.Issue.Name
                    }).ToList();
        }

        public IList<ResourceDTO> GetResources()
        {
            return Context.Resources.Select(x => new ResourceDTO
            {
                Id = x.Id,
                Title = x.Title
            }).ToList();
        }

        public IList<ResourceDTO> GetResourcesByPlan(int planId)
        {
            var submittedStepResult = GetSubmittedDefinitiveStepResult(planId, Steps.ActionPlanKeyQuestions);

            if (submittedStepResult == null)
            {
                return new List<ResourceDTO>();
            }

            var issueOptions = submittedStepResult.IssueOptionAnswers.Where(x => x.IsBestOption);

            var resources = new List<Resource>();

            foreach (var resource in issueOptions.SelectMany(x => x.IssueOptionAnswersToResources.Select(s => s.Resource)))
            {
                if (!resources.Any(x => x.Id == resource.Id))
                {
                    resources.Add(resource);
                }
            }
            return resources.Select(x => new ResourceDTO
            {
                Id = x.Id,
                Title = x.Title
            }).ToList();
        }

        #region Private methods

        #region General methods

        private bool IsAvailableStep(int planId, string stepIndex)
        {
            var plan = Get(planId);

            if ((stepIndex == Steps.ActionPlanKeyQuestions || stepIndex == Steps.ActionPlanDetailed || stepIndex == Steps.Review)
                && plan.IsWithActionPlan.HasValue && !plan.IsWithActionPlan.Value)
            {
                return true;
            }

            var stepTask = GetStepTask(planId, stepIndex);

            if (stepTask.IsCompleted)
            {
                return true;
            }

            var sortedStepArray = GetStepList().ToArray();

            var index = Array.IndexOf(sortedStepArray, stepIndex);

            if (index == 0)
            {
                return true;
            }

            var previousStep = sortedStepArray[index - 1];

            var previousStepTask = GetStepTask(planId, stepIndex);

            return previousStepTask.IsCompleted;
        }

        private IList<StepTaskDTO> GetStepTasks(int planId)
        {
            return Context.StepTasks.Where(x => x.PlanId == planId).ToList().Select(x =>
            {
                StepTaskStatus status;
                if (x.Schedule != null)
                {

                    if (x.IsCompleted)
                    {
                        if (x.Schedule >= DateTime.Now)
                        {
                            status = StepTaskStatus.Complete;
                        }
                        else
                        {
                            status = StepTaskStatus.OverdueComplete;
                        }
                    }
                    else
                    {
                        if (x.Schedule >= DateTime.Now)
                        {
                            status = StepTaskStatus.Incomplete;
                        }
                        else
                        {
                            status = StepTaskStatus.OverdueIncomplete;
                        }
                    }
                }
                else
                {
                    status = StepTaskStatus.Incomplete;
                }

                return new StepTaskDTO
                {
                    Id = x.Id,
                    PlanId = x.PlanId,
                    IsCompleted = x.IsCompleted,
                    RemindIn = x.Remind,
                    Schedule = x.Schedule,
                    Step = x.Step,
                    Status = status
                };
            }).OrderBy(x => Array.IndexOf(GetStepList().ToArray(), x.Step)).ToList();
        }

        private UserStepResult GetOrCreateUserStepResult(int planId, string stepIndex, bool isDefinitive, int userId)
        {
            UserStepResult userStepResult;

            if (isDefinitive)
            {
                userStepResult = GetFinalDefinitiveStepResult(planId, stepIndex);
            }
            else
            {
                userStepResult = GetUserStepResult(planId, stepIndex, userId);
            }

            if (userStepResult == null)
            {
                userStepResult = CreateUserStepResult(planId, stepIndex, isDefinitive, userId);
            }

            return userStepResult;
        }

        private UserStepResult CreateUserStepResult(int planId, string stepIndex, bool isDefinitive, int userId)
        {
            var userStepResult = new UserStepResult
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = userId,
                UpdatedBy = userId,
                IsDefinitive = isDefinitive,
                PlanId = planId,
                Step = stepIndex
            };

            if (isDefinitive)
            {
                userStepResult.IsFinal = true;
            }
            else
            {
                userStepResult.PlanId = null;
                var userToPlan = Context.UsersToPlans.Where(x => x.UserId == userId && x.PlanId == planId).FirstOrDefault();
                userStepResult.UserToPlan = userToPlan;
            }

            Context.UserStepResults.Add(userStepResult);
            Context.SaveChanges();
            return userStepResult;
        }

        private IList<UserStepResult> GetUserStepResultByCondition(Expression<Func<UserStepResult, bool>> expresion)
        {
            return Context.UserStepResults
                    .Include(x => x.UserToPlan).ThenInclude(x => x.User)
                    .Where(expresion)
                    .Include(x => x.BooleanAnswers)
                    .Include(x => x.SelectAnswers).ThenInclude(x => x.Option)
                    .Include(x => x.TextAnswers)
                    .Include(x => x.FileAnswers).ThenInclude(x => x.File)
                    .Include(x => x.StepTaskAnswers).ThenInclude(x => x.StepTask)
                    .Include(x => x.StepTaskAnswers).ThenInclude(x => x.UserToPlan)
                    .Include(x => x.ValueAnswers)
                    .Include(x => x.StakeholderAnswers).ThenInclude(x => x.Category)
                    .Include(x => x.SWOTAnswers)
                    .Include(x => x.StrategicIssueAnswers).ThenInclude(x => x.Issue)
                    .Include(x => x.StakeholderRatingAnswers).ThenInclude(x => x.Criteria)
                    .Include(x => x.StakeholderRatingAnswers).ThenInclude(x => x.Stakeholder)
                    .Include(x => x.IssueOptionAnswers).ThenInclude(x => x.IssueOptionAnswersToResources).ThenInclude(x => x.Resource)
                    .Include(x => x.IssueOptionAnswers).ThenInclude(x => x.Issue)
                    .Include(x => x.PreparingAnswers).ThenInclude(x => x.IssueOptionAnswer.Issue).ToList();
        }

        private UserStepResult GetUserStepResult(int planId, string stepIndex, int userId)
        {
            return GetUserStepResultByCondition(x => x.Step == stepIndex && x.UserToPlan.UserId == userId && x.UserToPlan.PlanId == planId).SingleOrDefault();
        }

        private UserStepResult GetSubmittedDefinitiveStepResult(int planId, string stepIndex)
        {
            return GetUserStepResultByCondition(x => x.Step == stepIndex && x.PlanId == planId && x.IsDefinitive && x.IsSubmitted).SingleOrDefault();
        }

        private UserStepResult GetFinalDefinitiveStepResult(int planId, string stepIndex)
        {
            return GetUserStepResultByCondition(x => x.Step == stepIndex && x.PlanId == planId && x.IsDefinitive && x.IsFinal.HasValue && x.IsFinal.Value)
                    .SingleOrDefault();
        }

        private IList<UserStepResult> GetPlanStepResults(int planId, string stepIndex)
        {
            var userToPlans = Context.UsersToPlans
               .Where(x => x.PlanId == planId)
               .ToList();

            var userStepResults = new List<UserStepResult>();

            foreach (var userToPlan in userToPlans)
            {
                var userStepResult = GetUserStepResult(planId, stepIndex, userToPlan.UserId);
                if (userStepResult != null)
                {
                    userStepResults.Add(userStepResult);
                }
            }

            return userStepResults;
        }

        private bool DeleteUserStepResult(int id)
        {
            var userStepResult = GetUserStepResultByCondition(x => x.Id == id).SingleOrDefault();

            if (userStepResult != null)
            {
                Context.BooleanAnswers.RemoveRange(userStepResult.BooleanAnswers);
                Context.SelectAnswers.RemoveRange(userStepResult.SelectAnswers);
                Context.ValueAnswers.RemoveRange(userStepResult.ValueAnswers);
                Context.StrategicIssueAnswers.RemoveRange(userStepResult.StrategicIssueAnswers);
                Context.PreparingAnswers.RemoveRange(userStepResult.PreparingAnswers);
                Context.IssueOptionAnswers.RemoveRange(userStepResult.IssueOptionAnswers);
                Context.ValueAnswers.RemoveRange(userStepResult.ValueAnswers);
                Context.FileAnswers.RemoveRange(userStepResult.FileAnswers);
                Context.SWOTAnswers.RemoveRange(userStepResult.SWOTAnswers);
                Context.StakeholderRatingAnswers.RemoveRange(userStepResult.StakeholderRatingAnswers);
                Context.StakeholderAnswers.RemoveRange(userStepResult.StakeholderAnswers);
                Context.StepTaskAnswers.RemoveRange(userStepResult.StepTaskAnswers);
                Context.UserStepResults.Remove(userStepResult);

                Context.SaveChanges();

                return true;
            }
            return false;
        }

        private IList<StepBlockDTO> GetStepStructure(string stepIndex)
        {
            return Context.StepBlocks.Where(x => x.Step == stepIndex)
                .Include(x => x.Questions)
                .ThenInclude(x => x.Options)
                .Include(x => x.Questions)
                .ThenInclude(x => x.Files)
                .OrderBy(x => x.Order)
                .AsEnumerable()
                .Select(x => Mapper.Map<StepBlockDTO>(x))
                .ToList();
        }

        private StepTask GetOrCreateStepTask(int planId, string stepIndex, int userId)
        {
            var stepTask = GetStepTask(planId, stepIndex);

            if (stepTask == null)
            {
                stepTask = new StepTask
                {
                    PlanId = planId,
                    Step = stepIndex,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    IsCompleted = false
                };

                Context.StepTasks.Add(stepTask);
                Context.SaveChanges();
            }
            return stepTask;
        }

        private StepTask GetStepTask(int planId, string stepIndex)
        {
            return Context.StepTasks.Where(x => x.PlanId == planId && x.Step == stepIndex).SingleOrDefault();
        }

        private PlanStepDTO GetPlanStep(int planId, string stepIndex, bool isDefinitive)
        {
            var steptask = GetStepTask(planId, stepIndex);

            var blocksDTOs = GetStepStructure(stepIndex);

            var planStep = new PlanStepDTO
            {
                PlanId = planId,
                Step = stepIndex,
                StepBlocks = blocksDTOs.ToList(),
                PlanningTeam = GetPlanningTeam(planId),
                IsAdmin = isDefinitive,
                IsCompleted = steptask?.IsCompleted ?? false,
                StepTasks = GetStepTasks(planId)
            };

            if (stepIndex == Steps.IssuesDistinguish)
            {
                planStep.AdditionalQuestions = GetIssueDistinguishQuestions();
            }

            if (isDefinitive)
            {
                var involvedUsers = Context.UsersToPlans
                    .Where(x => x.PlanId == planId)
                    .Include(x => x.User);

                var userStepResults = Context.UserStepResults.Include(x => x.UserToPlan).Where(x => x.Step == stepIndex && !x.IsDefinitive && x.UserToPlan.PlanId == planId).ToList();

                planStep.SubmittedUsers = involvedUsers.Where(involvedUser => userStepResults.Any(x => x.UserToPlanId == involvedUser.Id && x.IsSubmitted))
                    .Select(x => new UserPlanningMemberDTO
                    {
                        FullName = $"{x.User.FirstName} {x.User.LastName}"
                    });

                planStep.NotSubmittedUsers = involvedUsers.Where(involvedUser => !userStepResults.Any(x => x.UserToPlanId == involvedUser.Id && x.IsSubmitted))
                   .Select(x => new UserPlanningMemberDTO
                   {
                       FullName = $"{x.User.FirstName} {x.User.LastName}"
                   });
            }
            return planStep;
        }

        private IList<QuestionDTO> GetIssueDistinguishQuestions()
        {
            return Context.Questions.Where(x => x.Type == QuestionTypes.IssueDistinguishMultiSelect || x.Type == QuestionTypes.IssueDistinguishSelect || x.Type == QuestionTypes.IssueDistinguishTypeSelect)
                        .Include(x => x.Options).AsEnumerable().Select(x => Mapper.Map<QuestionDTO>(x)).ToList();
        }

        #endregion

        #region Saving methods

        private void SaveStepTasks(IList<StepTaskDTO> stepTasks, int userId)
        {
            if (stepTasks == null)
            {
                return;
            }

            foreach (var stepTask in stepTasks)
            {
                var dbStepTask = Context.StepTasks.Where(x => x.Id == stepTask.Id).SingleOrDefault();
                if (dbStepTask.Schedule != stepTask.Schedule || dbStepTask.Remind != stepTask.RemindIn)
                {
                    dbStepTask.Schedule = stepTask.Schedule;
                    dbStepTask.Remind = stepTask.RemindIn;
                    dbStepTask.UpdatedAt = DateTime.Now;
                    dbStepTask.UpdatedBy = userId;
                }
            }
            Context.SaveChanges();
        }

        private void SaveAnswers(IList<AnswerGroupDTO> answerGroups, UserStepResult userStepResult)
        {
            foreach (var answerGroup in answerGroups)
            {
                var question = Context.Questions.Where(x => x.Id == answerGroup.QuestionId).FirstOrDefault();

                switch (question.Type)
                {
                    case QuestionTypes.Boolean:
                        SaveBooleanAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.Select:
                        SaveSelectAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.TagMultiSelect:
                        SaveTagMultiSelectAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.PlanTypeSelect:
                        SaveBooleanAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.TextArea:
                        SaveTextAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.File:
                        SaveFileAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.Values:
                        SaveValueAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.Stakeholder:
                        SaveStakeholderAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.SWOT:
                        SaveSwotAnswers(answerGroup, userStepResult); break;

                    case QuestionTypes.StrategicIssues:
                        SaveStrategicIssueAnswers(answerGroup, userStepResult); break;

                    case QuestionTypes.InternalStakeholdersRating:
                        SaveStakeholdersRatingAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.ExternalStakeholdersRating:
                        SaveStakeholdersRatingAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.IssueOptions:
                        SaveIssueOptionAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.IssueDistinguish:
                        SaveIssueDistinguishAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.IssuePreparing:
                        SaveIssuePreparingAnswer(answerGroup, userStepResult); break;

                    case QuestionTypes.ResourceReview:
                        SaveResourceReviewAnswers(answerGroup, userStepResult); break;

                    case QuestionTypes.RateSlider:
                        SaveTextAnswer(answerGroup, userStepResult); break;
                }
            }
        }

        private void SaveIssuePreparingAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<PreparingAnswer> dbAnswers = null;

            dbAnswers = userStepResult.PreparingAnswers.Where(x => x.QuestionId == answerGroup.QuestionId && x.CreatedBy == userStepResult.UpdatedBy).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                var updateAnswer = answerGroup.Answer.IssuePreparingAnswers.Where(x => x.IssueOptionAnswerId == dbAnswer.IssueOptionAnswerId).FirstOrDefault();

                if (updateAnswer == null)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.PreparingAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
                else if (dbAnswer.IssueOptionAnswerId != updateAnswer.IssueOptionAnswerId ||
                        !dbAnswer.Date.Equals(updateAnswer.Date) ||
                        !dbAnswer.HowItWillBeDone.Equals(updateAnswer.HowItWillBeDone) ||
                        dbAnswer.IsCompleted != updateAnswer.IsCompleted)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    dbAnswer.IssueOptionAnswerId = updateAnswer.IssueOptionAnswerId;
                    dbAnswer.Date = updateAnswer.Date;
                    dbAnswer.HowItWillBeDone = updateAnswer.HowItWillBeDone ?? string.Empty;
                    dbAnswer.IsCompleted = updateAnswer.IsCompleted;
                    dbAnswer.UpdatedAt = userStepResult.UpdatedAt;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }

            foreach (var answer in answerGroup.Answer.IssuePreparingAnswers)
            {
                if (!dbAnswers.Any(x => x.IssueOptionAnswerId == answer.IssueOptionAnswerId))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    var newAnswer = new PreparingAnswer
                    {
                        QuestionId = answerGroup.QuestionId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        UpdatedBy = userStepResult.UpdatedBy,
                        IssueOptionAnswerId = answer.IssueOptionAnswerId,
                        Date = answer.Date,
                        HowItWillBeDone = answer.HowItWillBeDone ?? string.Empty,
                        IsCompleted = answer.IsCompleted
                    };

                    userStepResult.PreparingAnswers.Add(newAnswer);
                }
            }
        }

        private void SaveBooleanAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            BooleanAnswer dbAnswer = null;

            dbAnswer = userStepResult.BooleanAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).FirstOrDefault();

            if (dbAnswer != null)
            {
                if (dbAnswer.Answer != answerGroup.Answer.BooleanAnswer)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    dbAnswer.Answer = answerGroup.Answer.BooleanAnswer;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }
            else
            {
                userStepResult.UpdatedAt = DateTime.Now;

                var newAnswer = new BooleanAnswer
                {
                    Answer = answerGroup.Answer.BooleanAnswer,
                    QuestionId = answerGroup.QuestionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userStepResult.UpdatedBy,
                    UpdatedBy = userStepResult.UpdatedBy
                };

                userStepResult.BooleanAnswers.Add(newAnswer);
            }

        }

        private void SaveSelectAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            SelectAnswer dbAnswer = null;

            dbAnswer = userStepResult.SelectAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).FirstOrDefault();

            if (answerGroup.Answer.SelectAnswer.OptionId < 1)
            {
                answerGroup.Answer.SelectAnswer.OptionId = null;
            }

            if (answerGroup.Answer.SelectAnswer.OptionId != null)
            {
                answerGroup.Answer.SelectAnswer.AltOption = null;
            }

            if (dbAnswer != null)
            {
                if (!(dbAnswer.OptionId == answerGroup.Answer.SelectAnswer.OptionId
                    && dbAnswer.AltOption == answerGroup.Answer.SelectAnswer.AltOption))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    dbAnswer.OptionId = answerGroup.Answer.SelectAnswer.OptionId;
                    dbAnswer.AltOption = answerGroup.Answer.SelectAnswer.AltOption;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }
            else
            {
                userStepResult.UpdatedAt = DateTime.Now;

                var newAnswer = new SelectAnswer
                {
                    OptionId = answerGroup.Answer.SelectAnswer.OptionId,
                    AltOption = answerGroup.Answer.SelectAnswer.AltOption,
                    QuestionId = answerGroup.QuestionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userStepResult.UpdatedBy,
                    UpdatedBy = userStepResult.UpdatedBy,
                    IssueId = answerGroup.Answer.SelectAnswer.IssueId
                };

                userStepResult.SelectAnswers.Add(newAnswer);
            }

        }

        private void SaveTagMultiSelectAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<SelectAnswer> dbAnswers = null;

            dbAnswers = userStepResult.SelectAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                var answerText = dbAnswer.Option != null ? dbAnswer.Option.Title : dbAnswer.AltOption;

                if (answerGroup.Answer == null || !answerGroup.Answer.TagSelectAnswers.Contains(answerText))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.SelectAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
            }

            if (answerGroup.Answer == null) return;

            foreach (var answer in answerGroup.Answer.TagSelectAnswers)
            {
                if (!dbAnswers.Any(x => x.Option?.Title == answer || x.AltOption == answer))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    var dbOption = Context.Options.Where(x => x.QuestionId == answerGroup.QuestionId && x.Title == answer).FirstOrDefault();

                    var newAnswer = new SelectAnswer
                    {
                        QuestionId = answerGroup.QuestionId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        UpdatedBy = userStepResult.UpdatedBy
                    };

                    if (dbOption != null)
                    {
                        newAnswer.Option = dbOption;
                    }
                    else
                    {
                        newAnswer.AltOption = answer;
                    }

                    userStepResult.SelectAnswers.Add(newAnswer);
                }
            }
        }

        private void SaveMultiSelectAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<SelectAnswer> dbAnswers = null;

            dbAnswers = userStepResult.SelectAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                if (answerGroup.Answer.MultiSelectAnswer?.SelectAnswers == null
                    || !answerGroup.Answer.MultiSelectAnswer.SelectAnswers.Contains(dbAnswer.OptionId.Value))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.SelectAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
            }

            if (answerGroup.Answer.MultiSelectAnswer?.SelectAnswers == null)
            {
                return;
            }

            foreach (var answer in answerGroup.Answer.MultiSelectAnswer.SelectAnswers)
            {
                if (!dbAnswers.Any(x => x.OptionId.Value == answer))
                {
                    var dbOption = Context.Options.Where(x => x.QuestionId == answerGroup.QuestionId && x.Id == answer).SingleOrDefault();

                    if (dbOption != null)
                    {
                        userStepResult.UpdatedAt = DateTime.Now;

                        var newAnswer = new SelectAnswer
                        {
                            QuestionId = answerGroup.QuestionId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CreatedBy = userStepResult.UpdatedBy,
                            UpdatedBy = userStepResult.UpdatedBy,
                            Option = dbOption,
                            IssueId = answerGroup.Answer.MultiSelectAnswer.IssueId
                        };
                        userStepResult.SelectAnswers.Add(newAnswer);
                    }

                }
            }
        }

        private void SaveTextAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            TextAnswer dbAnswer = null;

            dbAnswer = userStepResult.TextAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).FirstOrDefault();

            if (dbAnswer != null)
            {
                if (!dbAnswer.Text.Equals(answerGroup.Answer.TextAnswer.Text))
                {
                    userStepResult.UpdatedAt = DateTime.Now;
                    dbAnswer.Text = answerGroup.Answer.TextAnswer.Text ?? String.Empty;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }
            else
            {
                userStepResult.UpdatedAt = DateTime.Now;

                var newAnswer = new TextAnswer
                {
                    Text = answerGroup.Answer.TextAnswer.Text ?? String.Empty,
                    QuestionId = answerGroup.QuestionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userStepResult.UpdatedBy,
                    UpdatedBy = userStepResult.UpdatedBy
                };

                userStepResult.TextAnswers.Add(newAnswer);
            }
        }

        private void SaveFileAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<FileAnswer> dbAnswers = null;

            dbAnswers = userStepResult.FileAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                var answerFileId = dbAnswer.FileId;

                if (answerGroup.Answer == null || !answerGroup.Answer.InputFileAnswer.Contains(answerFileId))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.FileAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
            }

            if (answerGroup.Answer == null) return;

            foreach (var answer in answerGroup.Answer.InputFileAnswer)
            {
                if (!dbAnswers.Any(x => x.FileId == answer))
                {
                    var dbFile = Context.Files.Where(x => x.Id == answer).FirstOrDefault();

                    if (dbFile == null) continue;

                    userStepResult.UpdatedAt = DateTime.Now;

                    var newAnswer = new FileAnswer
                    {
                        QuestionId = answerGroup.QuestionId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        UpdatedBy = userStepResult.UpdatedBy,
                        File = dbFile
                    };

                    userStepResult.FileAnswers.Add(newAnswer);
                }
            }
        }

        private void SaveStepTaskAnswers(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            var dbStepTaskAnswers = userStepResult.StepTaskAnswers.ToList();

            foreach (var dbStepTaskAnswer in dbStepTaskAnswers)
            {
                if (answerGroup?.Answer.StepTaskAnswers == null || !answerGroup.Answer.StepTaskAnswers.Any(x => x.Id == dbStepTaskAnswer.Id))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.StepTaskAnswers.Remove(dbStepTaskAnswer);
                }
            }

            Context.SaveChanges();

            if (answerGroup?.Answer?.StepTaskAnswers == null)
            {
                return;
            }

            int planId = 0;

            if (userStepResult.IsDefinitive)
            {
                planId = userStepResult.PlanId.Value;
            }
            else
            {
                planId = userStepResult.UserToPlan.PlanId;
            }

            var stepTasks = Context.StepTasks.Where(x => x.PlanId == planId).ToList();

            foreach (var steptaskAnswer in answerGroup.Answer.StepTaskAnswers.Where(x => x.Id == 0))
            {
                userStepResult.UpdatedAt = DateTime.Now;

                var newAnswer = new StepTaskAnswer
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userStepResult.UpdatedBy,
                    UpdatedBy = userStepResult.UpdatedBy,
                    Email = steptaskAnswer.Email,
                    FirstName = steptaskAnswer.FirstName,
                    LastName = steptaskAnswer.LastName,
                    UserToPlanId = steptaskAnswer.UserToPlanId,
                    StepTask = stepTasks.Where(x => x.Step == steptaskAnswer.Step).SingleOrDefault()
                };

                userStepResult.StepTaskAnswers.Add(newAnswer);
            }

        }

        private void SaveValueAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<ValueAnswer> dbAnswers = null;

            dbAnswers = userStepResult.ValueAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                var updateAnswer = answerGroup.Answer?.ValueAnswer.Where(x => x.Id == dbAnswer.Id).FirstOrDefault();

                if (updateAnswer == null)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.ValueAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
                else if (!dbAnswer.Value.Equals(updateAnswer.Value) ||
                        !dbAnswer.Definition.Equals(updateAnswer.Definition) ||
                        !dbAnswer.Description.Equals(updateAnswer.Description))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    dbAnswer.Value = updateAnswer.Value;
                    dbAnswer.Definition = updateAnswer.Definition;
                    dbAnswer.Description = updateAnswer.Description;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }

            if (answerGroup.Answer == null) return;

            foreach (var answer in answerGroup.Answer.ValueAnswer)
            {
                if (!dbAnswers.Any(x => x.Id == answer.Id))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    var newAnswer = new ValueAnswer
                    {
                        QuestionId = answerGroup.QuestionId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        UpdatedBy = userStepResult.UpdatedBy,
                        Value = answer.Value,
                        Definition = answer.Definition,
                        Description = answer.Description
                    };

                    userStepResult.ValueAnswers.Add(newAnswer);
                }
            }
        }

        private void SaveStakeholderAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<StakeholderAnswer> dbAnswers = null;

            dbAnswers = userStepResult.StakeholderAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                var updateAnswer = answerGroup.Answer?.StakeholderAnswers.Where(x => x.Id == dbAnswer.Id).FirstOrDefault();

                if (updateAnswer == null)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.StakeholderAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
            }

            if (answerGroup.Answer == null) return;

            foreach (var answer in answerGroup.Answer.StakeholderAnswers)
            {
                if (!dbAnswers.Any(x => x.Id == answer.Id))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    StakeholderAnswer newAnswer = new StakeholderAnswer
                    {
                        QuestionId = answerGroup.QuestionId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        UpdatedBy = userStepResult.UpdatedBy,
                        UserId = answer.UserId,
                        CategoryId = answer.CategoryId,
                    };

                    if (newAnswer.UserId.HasValue)
                    {
                        var user = Context.Users.Find(newAnswer.UserId.Value);

                        if (user == null) continue;

                        newAnswer.FirstName = user.FirstName;
                        newAnswer.LastName = user.LastName;
                        newAnswer.IsInternal = true;
                    }
                    else
                    {
                        newAnswer.FirstName = answer.FirstName;
                        newAnswer.LastName = answer.LastName;
                        newAnswer.Email = answer.Email;
                    }

                    userStepResult.StakeholderAnswers.Add(newAnswer);
                }
            }
        }

        private void SaveSwotAnswers(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<SWOTAnswer> dbAnswers = null;

            dbAnswers = userStepResult.SWOTAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            if (answerGroup.Answer != null)
            {
                answerGroup.Answer.SwotAnswer.Strengths = answerGroup.Answer.SwotAnswer.Strengths?.Distinct().ToList() ?? Enumerable.Empty<string>().ToList();
                answerGroup.Answer.SwotAnswer.Opportunities = answerGroup.Answer.SwotAnswer.Opportunities?.Distinct().ToList() ?? Enumerable.Empty<string>().ToList();
                answerGroup.Answer.SwotAnswer.Threats = answerGroup.Answer.SwotAnswer.Threats?.Distinct().ToList() ?? Enumerable.Empty<string>().ToList();
                answerGroup.Answer.SwotAnswer.Weaknesses = answerGroup.Answer.SwotAnswer.Weaknesses?.Distinct().ToList() ?? Enumerable.Empty<string>().ToList();
            }

            foreach (var dbAnswer in dbAnswers)
            {
                var isContained = false;
                if (dbAnswer.Type == SWOTTypes.Strength)
                {
                    isContained = answerGroup.Answer?.SwotAnswer.Strengths.Contains(dbAnswer.Name) ?? false;
                }
                if (dbAnswer.Type == SWOTTypes.Weakness)
                {
                    isContained = answerGroup.Answer?.SwotAnswer.Weaknesses.Contains(dbAnswer.Name) ?? false;
                }
                if (dbAnswer.Type == SWOTTypes.Opportunity)
                {
                    isContained = answerGroup.Answer?.SwotAnswer.Opportunities.Contains(dbAnswer.Name) ?? false;
                }
                if (dbAnswer.Type == SWOTTypes.Threat)
                {
                    isContained = answerGroup.Answer?.SwotAnswer.Threats.Contains(dbAnswer.Name) ?? false;
                }

                if (!isContained)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.SWOTAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
            }

            if (answerGroup.Answer == null) return;

            var swotAnswers = new List<SWOTAnswer>();
            swotAnswers.AddRange(answerGroup.Answer.SwotAnswer.Strengths?.Select(x => new SWOTAnswer { Type = SWOTTypes.Strength, Name = x }));
            swotAnswers.AddRange(answerGroup.Answer.SwotAnswer.Threats?.Select(x => new SWOTAnswer { Type = SWOTTypes.Threat, Name = x }));
            swotAnswers.AddRange(answerGroup.Answer.SwotAnswer.Opportunities?.Select(x => new SWOTAnswer { Type = SWOTTypes.Opportunity, Name = x }));
            swotAnswers.AddRange(answerGroup.Answer.SwotAnswer.Weaknesses?.Select(x => new SWOTAnswer { Type = SWOTTypes.Weakness, Name = x }));

            foreach (var answer in swotAnswers)
            {
                if (!dbAnswers.Any(x => x.Name == answer.Name && x.Type == answer.Type))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    answer.QuestionId = answerGroup.QuestionId;
                    answer.CreatedAt = DateTime.Now;
                    answer.UpdatedAt = DateTime.Now;
                    answer.CreatedBy = userStepResult.UpdatedBy;
                    answer.UpdatedBy = userStepResult.UpdatedBy;
                    answer.IsIssue = answer.Type == SWOTTypes.Threat || answer.Type == SWOTTypes.Weakness ? true : false;
                    userStepResult.SWOTAnswers.Add(answer);
                }
            }
        }

        private void SaveStrategicIssueAnswers(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            if (answerGroup.Answer?.StrategicIssueAnswers == null)
            {
                return;
            }
            var dbAnswers = userStepResult.StrategicIssueAnswers.Where(x => x.QuestionId == answerGroup.QuestionId);

            foreach (var strategicIssueAnswer in answerGroup.Answer.StrategicIssueAnswers)
            {
                var dbAnswer = dbAnswers.Where(x => x.IssueId == strategicIssueAnswer.IssueId).SingleOrDefault();
                if (dbAnswer != null)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    dbAnswer.Goal = strategicIssueAnswer.Goal ?? string.Empty;
                    dbAnswer.Result = strategicIssueAnswer.Result ?? string.Empty;
                    dbAnswer.Solution = strategicIssueAnswer.Solution ?? string.Empty;
                    dbAnswer.Why = strategicIssueAnswer.Why ?? string.Empty;
                    dbAnswer.Ranking = strategicIssueAnswer.Ranking;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
                else
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    dbAnswer = new StrategicIssueAnswer
                    {
                        Goal = strategicIssueAnswer.Goal ?? string.Empty,
                        Result = strategicIssueAnswer.Result ?? string.Empty,
                        Solution = strategicIssueAnswer.Solution ?? string.Empty,
                        Why = strategicIssueAnswer.Why ?? string.Empty,
                        Ranking = strategicIssueAnswer.Ranking,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = userStepResult.UpdatedBy,
                        CreatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        IssueId = strategicIssueAnswer.IssueId,
                        QuestionId = answerGroup.QuestionId
                    };

                    userStepResult.StrategicIssueAnswers.Add(dbAnswer);
                }
            }
        }

        private void SaveStakeholdersRatingAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<StakeholderRatingAnswer> dbAnswers = null;

            dbAnswers = userStepResult.StakeholderRatingAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                var updateAnswer = answerGroup.Answer?.StakeholderRatingAnswers.Where(x => x.StakeholderId == dbAnswer.StakeholderId && dbAnswer.CreatedBy == userStepResult.UpdatedBy).FirstOrDefault();

                if (updateAnswer == null)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.StakeholderRatingAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
                else if (
                        dbAnswer.Priority != updateAnswer.Priority ||
                        dbAnswer.Criteria.Any(x => updateAnswer.CriterionsRates.Where(y => y.CriterionId == x.CriterionId && y.Rate != x.Rate).Count() > 0)
                    )
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    dbAnswer.Priority = updateAnswer.Priority;
                    dbAnswer.Criteria = updateAnswer.CriterionsRates.Select(x => new StakeholderRatingAnswerToDictionary
                    {
                        CriterionId = x.CriterionId,
                        Rate = x.Rate
                    }).ToList();
                    dbAnswer.Grade = (double)updateAnswer.CriterionsRates.Sum(x => x.Rate) / updateAnswer.CriterionsRates.Count();
                    dbAnswer.UpdatedAt = userStepResult.UpdatedAt;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }

            if (answerGroup.Answer == null) return;

            foreach (var answer in answerGroup.Answer.StakeholderRatingAnswers)
            {
                if (!dbAnswers.Any(x => x.StakeholderId == answer.StakeholderId && x.CreatedBy == userStepResult.UpdatedBy))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    StakeholderRatingAnswer newAnswer = new StakeholderRatingAnswer
                    {
                        QuestionId = answerGroup.QuestionId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        UpdatedBy = userStepResult.UpdatedBy,
                        StakeholderId = answer.StakeholderId,
                        Priority = answer.Priority,
                        Criteria = answer.CriterionsRates.Select(x => new StakeholderRatingAnswerToDictionary
                        {
                            CriterionId = x.CriterionId,
                            Rate = x.Rate
                        }).ToList(),
                        Grade = (double)answer.CriterionsRates.Sum(x => x.Rate) / answer.CriterionsRates.Count()
                    };

                    userStepResult.StakeholderRatingAnswers.Add(newAnswer);
                }
            }
        }

        private void SaveIssueOptionAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            IList<IssueOptionAnswer> dbAnswers = null;

            dbAnswers = userStepResult.IssueOptionAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).ToList();

            foreach (var dbAnswer in dbAnswers)
            {
                var updateAnswer = answerGroup.Answer?.IssueOptionAnswers.Where(x => x.Id == dbAnswer.Id).FirstOrDefault();

                if (updateAnswer == null)
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    Context.IssueOptionAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                }
                else
                {
                    var resources = updateAnswer.Resources?.Split(',') ?? Enumerable.Empty<string>();

                    if (dbAnswer.IssueId != updateAnswer.IssueId ||
                          dbAnswer.IsBestOption != updateAnswer.IsBestOption ||
                          !(dbAnswer.Actors == updateAnswer.Actors) ||
                          !(dbAnswer.Option == updateAnswer.Option) ||
                          dbAnswer.IssueOptionAnswersToResources.Count() != resources.Count() ||
                          dbAnswer.IssueOptionAnswersToResources.Select(x => x.IssueOptionAnswer.Issue).Any(x => !resources.Contains(x.Name))
                       )
                    {
                        userStepResult.UpdatedAt = DateTime.Now;

                        dbAnswer.IssueId = updateAnswer.IssueId;
                        dbAnswer.IsBestOption = updateAnswer.IsBestOption;
                        dbAnswer.Actors = updateAnswer.Actors;
                        dbAnswer.Option = updateAnswer.Option;
                        dbAnswer.IssueOptionAnswersToResources = InitIssueResources(updateAnswer.Resources, dbAnswer, userStepResult.UpdatedBy);
                        dbAnswer.UpdatedAt = userStepResult.UpdatedAt;
                        dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                    }
                }
            }

            if (answerGroup.Answer == null) return;

            foreach (var answer in answerGroup.Answer.IssueOptionAnswers)
            {
                if (!dbAnswers.Any(x => x.Id == answer.Id))
                {
                    userStepResult.UpdatedAt = DateTime.Now;

                    IssueOptionAnswer newAnswer = new IssueOptionAnswer
                    {
                        QuestionId = answerGroup.QuestionId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = userStepResult.UpdatedBy,
                        UpdatedBy = userStepResult.UpdatedBy,
                        IssueId = answer.IssueId,
                        IsBestOption = answer.IsBestOption,
                        Actors = answer.Actors,
                        Option = answer.Option
                    };

                    newAnswer.IssueOptionAnswersToResources = InitIssueResources(answer.Resources, newAnswer, userStepResult.UpdatedBy);

                    userStepResult.IssueOptionAnswers.Add(newAnswer);
                }
            }
        }

        private IList<IssueOptionAnswerToResource> InitIssueResources(string resources, IssueOptionAnswer answer, int? userId)
        {
            IList<IssueOptionAnswerToResource> result = new List<IssueOptionAnswerToResource>();

            if (resources == null) return result;

            resources.Split(',').ToList().ForEach(y =>
            {
                var issueRes = answer.IssueOptionAnswersToResources.Where(x => x.Resource.Title == y).FirstOrDefault();

                if (issueRes == null)
                {
                    var res = Context.Resources.Where(x => x.Title == y).FirstOrDefault();

                    if (res == null)
                    {
                        res = new Resource { Title = y, CreatedAt = DateTime.Now, CreatedBy = userId };

                        Context.Resources.Add(res);
                        Context.SaveChanges();
                    }

                    issueRes = new IssueOptionAnswerToResource
                    {
                        Resource = res
                    };
                }

                result.Add(issueRes);
            });

            return result;
        }

        private void SaveIssueDistinguishAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            if (answerGroup.Answer?.IssueDistinguishAnswers == null)
            {
                return;
            }

            var questions = GetIssueDistinguishQuestions();

            var dbAnswers = userStepResult.SelectAnswers.Where(x => answerGroup.Answer.IssueDistinguishAnswers.Any(s => s.QuestionId == x.QuestionId));

            foreach (var issueDistinguishAnswer in answerGroup.Answer.IssueDistinguishAnswers)
            {
                var question = questions.Where(x => x.Id == issueDistinguishAnswer.QuestionId).Single();
                if (question.Type == QuestionTypes.IssueDistinguishSelect || question.Type == QuestionTypes.IssueDistinguishTypeSelect)
                {
                    var questionDbAnswer = dbAnswers.Where(x => x.IssueId == issueDistinguishAnswer.IssueId && x.QuestionId == issueDistinguishAnswer.QuestionId).SingleOrDefault();

                    if (questionDbAnswer != null)
                    {
                        userStepResult.UpdatedAt = DateTime.Now;

                        questionDbAnswer.UpdatedAt = DateTime.Now;
                        questionDbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                        questionDbAnswer.OptionId = issueDistinguishAnswer.SelectAnswer;
                    }
                    else
                    {
                        userStepResult.UpdatedAt = DateTime.Now;

                        questionDbAnswer = new SelectAnswer
                        {
                            QuestionId = issueDistinguishAnswer.QuestionId,
                            IssueId = issueDistinguishAnswer.IssueId,
                            OptionId = issueDistinguishAnswer.SelectAnswer,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = userStepResult.UpdatedBy,
                            CreatedAt = DateTime.Now,
                            CreatedBy = userStepResult.UpdatedBy
                        };

                        userStepResult.SelectAnswers.Add(questionDbAnswer);
                    }
                }

                if (question.Type == QuestionTypes.IssueDistinguishMultiSelect)
                {
                    var questionDbAnswers = dbAnswers.Where(x => x.IssueId == issueDistinguishAnswer.IssueId && x.QuestionId == issueDistinguishAnswer.QuestionId).ToList();

                    foreach (var questionDbAnswer in questionDbAnswers)
                    {
                        if (!issueDistinguishAnswer.SelectAnswers.Contains(questionDbAnswer.OptionId.Value))
                        {
                            Context.SelectAnswers.Remove(questionDbAnswer);
                            Context.SaveChanges();
                        }
                    }

                    foreach (var selectAnswer in issueDistinguishAnswer.SelectAnswers)
                    {

                        if (!questionDbAnswers.Any(x => x.OptionId == selectAnswer) && selectAnswer > 0)
                        {
                            userStepResult.UpdatedAt = DateTime.Now;

                            var newAnswer = new SelectAnswer
                            {
                                QuestionId = issueDistinguishAnswer.QuestionId,
                                IssueId = issueDistinguishAnswer.IssueId,
                                OptionId = selectAnswer,
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = userStepResult.UpdatedBy,
                                CreatedAt = DateTime.Now,
                                CreatedBy = userStepResult.UpdatedBy
                            };

                            userStepResult.SelectAnswers.Add(newAnswer);
                        }
                    }
                }
            }
        }

        private void SaveResourceReviewAnswers(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            var dbBooleanAnswers = userStepResult.BooleanAnswers.Where(x => x.QuestionId == answerGroup.QuestionId);

            if (answerGroup.Answer?.ResourceReviewAnswers != null)
            {
                foreach (var answer in answerGroup.Answer.ResourceReviewAnswers)
                {
                    var dbBooleanAnswer = dbBooleanAnswers.Where(x => x.ResourceId == answer.ResourceId).SingleOrDefault();

                    if (dbBooleanAnswer != null)
                    {
                        if (dbBooleanAnswer.Answer != answer.Assured)
                        {
                            userStepResult.UpdatedAt = DateTime.Now;

                            dbBooleanAnswer.Answer = answer.Assured;
                            dbBooleanAnswer.UpdatedAt = DateTime.Now;
                            dbBooleanAnswer.UpdatedBy = userStepResult.UpdatedBy;
                        }
                    }
                    else
                    {
                        userStepResult.UpdatedAt = DateTime.Now;

                        dbBooleanAnswer = new BooleanAnswer
                        {
                            Answer = answer.Assured,
                            QuestionId = answerGroup.QuestionId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CreatedBy = userStepResult.UpdatedBy,
                            UpdatedBy = userStepResult.UpdatedBy,
                            ResourceId = answer.ResourceId
                        };

                        userStepResult.BooleanAnswers.Add(dbBooleanAnswer);
                    }
                }
            }

        }

        #endregion

        #region Reading methods

        private void FillWithAnswers(PlanStepDTO planStep, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            planStep.AnswerGroups = new List<AnswerGroupDTO>();

            for (int i = 0; i < planStep.StepBlocks.Count; i++)
            {
                var questions = planStep.StepBlocks[i].Questions;

                for (int j = 0; j < questions.Count; j++)
                {
                    switch (questions[j].Type)
                    {
                        case QuestionTypes.Boolean:
                            planStep.AnswerGroups.Add(GetBooleanAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.Select:
                            planStep.AnswerGroups.Add(GetSelectAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.TagMultiSelect:
                            planStep.AnswerGroups.Add(GetTagMultiSelectAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.PlanTypeSelect:
                            planStep.AnswerGroups.Add(GetBooleanAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.TextArea:
                            planStep.AnswerGroups.Add(GetTextAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.File:
                            planStep.AnswerGroups.Add(GetFileAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.Values:
                            planStep.AnswerGroups.Add(GetValueAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.Stakeholder:
                            planStep.AnswerGroups.Add(GetStakeholderAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.SWOT:
                            planStep.AnswerGroups.Add(GetSWOTAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.StrategicIssues:
                            planStep.AnswerGroups.Add(GetStrategicIssueAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.InternalStakeholdersRating:
                            planStep.AnswerGroups.Add(GetStakeholdersRatingAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.ExternalStakeholdersRating:
                            planStep.AnswerGroups.Add(GetStakeholdersRatingAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.IssueOptions:
                            planStep.AnswerGroups.Add(GetIssueOptionsAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.IssueDistinguish:
                            planStep.AnswerGroups.Add(GetIssueDistinguishAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.IssuePreparing:
                            planStep.AnswerGroups.Add(GetIssuePreparingAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.ResourceReview:
                            planStep.AnswerGroups.Add(GetResourceReviewAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;

                        case QuestionTypes.RateSlider:
                            planStep.AnswerGroups.Add(GetTextAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults)); break;
                    }
                }
            }

            if (planStep.Step == Steps.Predeparture)
            {
                planStep.StepTaskAnswers = GetStepTaskAnswers(currentUserStepResult, otherUserStepResults);
            }
        }

        private AnswerGroupDTO GetIssuePreparingAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.PreparingAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    IssuePreparingAnswers = currentUserAnswer.Select(x => new PreparingAnswerDTO
                    {
                        IssueOptionAnswerId = x.IssueOptionAnswerId,
                        Date = x.Date,
                        HowItWillBeDone = x.HowItWillBeDone,
                        IsCompleted = x.IsCompleted,
                        IssueName = x.IssueOptionAnswer.Issue.Name
                    }).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.PreparingAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    IssuePreparingAnswers = definitiveAnswer.Select(x => new PreparingAnswerDTO
                    {
                        IssueOptionAnswerId = x.IssueOptionAnswerId,
                        Date = x.Date,
                        HowItWillBeDone = x.HowItWillBeDone,
                        IsCompleted = x.IsCompleted,
                        IssueName = x.IssueOptionAnswer.Issue.Name
                    }).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.PreparingAnswers.Where(x => x.QuestionId == questionId);

                if (userAnswer != null && userAnswer.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        IssuePreparingAnswers = userAnswer.Select(x => new PreparingAnswerDTO
                        {
                            IssueOptionAnswerId = x.IssueOptionAnswerId,
                            Date = x.Date,
                            HowItWillBeDone = x.HowItWillBeDone,
                            IsCompleted = x.IsCompleted,
                            IssueName = x.IssueOptionAnswer.Issue.Name
                        }).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetBooleanAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.BooleanAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (currentUserAnswer != null)
            {
                answerGroup.Answer = new AnswerDTO { BooleanAnswer = currentUserAnswer.Answer };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.BooleanAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (definitiveAnswer != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO { BooleanAnswer = definitiveAnswer.Answer };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.BooleanAnswers.Where(a => a.QuestionId == questionId).SingleOrDefault();

                if (userAnswer != null)
                {
                    var answerDTO = new AnswerDTO
                    {
                        BooleanAnswer = userAnswer.Answer,
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetSelectAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.SelectAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (currentUserAnswer != null)
            {
                answerGroup.Answer = new AnswerDTO
                {
                    SelectAnswer = new SelectAnswerDTO
                    {
                        OptionId = currentUserAnswer.OptionId,
                        AltOption = currentUserAnswer.AltOption,
                        IssueId = currentUserAnswer.IssueId
                    }
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.SelectAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (definitiveAnswer != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    SelectAnswer = new SelectAnswerDTO
                    {
                        OptionId = definitiveAnswer.OptionId,
                        AltOption = definitiveAnswer.AltOption,
                        IssueId = definitiveAnswer.IssueId
                    }
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.SelectAnswers.Where(a => a.QuestionId == questionId).SingleOrDefault();

                if (userAnswer != null)
                {
                    var answerDTO = new AnswerDTO
                    {
                        SelectAnswer = new SelectAnswerDTO
                        {
                            OptionId = userAnswer.OptionId,
                            AltOption = userAnswer.AltOption,
                            IssueId = userAnswer.IssueId
                        },

                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }

            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetTagMultiSelectAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            answerGroup.Answer = new AnswerDTO
            {
                TagSelectAnswers = currentUserStepResult
                                    .SelectAnswers
                                    .Where(x => x.QuestionId == questionId)
                                    .Select(x => x.OptionId != null ? x.Option.Title : x.AltOption)
                                    .ToList()
            };

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            if (definitiveStepResult != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    TagSelectAnswers = definitiveStepResult
                                            .SelectAnswers
                                            .Where(x => x.QuestionId == questionId)
                                            .Select(x => x.OptionId != null ? x.Option.Title : x.AltOption)
                                            .ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswers = otherUserStepResult.SelectAnswers.Where(a => a.QuestionId == questionId).ToList();

                if (userAnswers.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        TagSelectAnswers = userAnswers.Select(s => s.OptionId != null ? s.Option.Title : s.AltOption).ToList(),

                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetMultiSelectAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            answerGroup.Answer = new AnswerDTO
            {
                MultiSelectAnswer = new MultiSelectAnswerDTO
                {
                    SelectAnswers = currentUserStepResult
                                    .SelectAnswers
                                    .Where(x => x.QuestionId == questionId)
                                    .Select(x => x.OptionId.Value)
                                    .ToList(),
                    IssueId = currentUserStepResult
                                    .SelectAnswers
                                    .Where(x => x.QuestionId == questionId).FirstOrDefault()?.IssueId
                }
            };

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            if (definitiveStepResult != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    MultiSelectAnswer = new MultiSelectAnswerDTO
                    {
                        SelectAnswers = definitiveStepResult
                                            .SelectAnswers
                                            .Where(x => x.QuestionId == questionId)
                                            .Select(x => x.OptionId.Value)
                                            .ToList(),
                        IssueId = definitiveStepResult
                                            .SelectAnswers
                                            .Where(x => x.QuestionId == questionId).FirstOrDefault()?.IssueId
                    }
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswers = otherUserStepResult.SelectAnswers.Where(a => a.QuestionId == questionId).ToList();

                if (userAnswers.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        MultiSelectAnswer = new MultiSelectAnswerDTO
                        {
                            SelectAnswers = userAnswers.Select(s => s.OptionId.Value).ToList(),
                            IssueId = userAnswers.FirstOrDefault().IssueId
                        },
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetTextAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.TextAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (currentUserAnswer != null)
            {
                answerGroup.Answer = new AnswerDTO { TextAnswer = new TextAnswerDTO { Text = currentUserAnswer.Text } };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.TextAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (definitiveAnswer != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO { TextAnswer = new TextAnswerDTO { Text = definitiveAnswer.Text } };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.TextAnswers.Where(a => a.QuestionId == questionId).SingleOrDefault();

                if (userAnswer != null)
                {
                    var answerDTO = new AnswerDTO
                    {
                        TextAnswer = new TextAnswerDTO { Text = userAnswer.Text },
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;
            return answerGroup;
        }

        private AnswerGroupDTO GetFileAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.FileAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    FileAnswer = currentUserAnswer.Select(x => new FileAnswerDTO
                    {
                        FileId = x.FileId,
                        Name = x.File?.Name,
                        Ext = x.File?.Ext,
                        Path = x.File?.Path
                    }).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.FileAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    FileAnswer = definitiveAnswer.Select(x => new FileAnswerDTO
                    {
                        FileId = x.FileId,
                        Name = x.File?.Name,
                        Ext = x.File?.Ext,
                        Path = x.File?.Path
                    }).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.FileAnswers.Where(x => x.QuestionId == questionId);

                if (userAnswer != null && userAnswer.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        FileAnswer = userAnswer.Select(x => new FileAnswerDTO
                        {
                            FileId = x.FileId,
                            Name = x.File?.Name,
                            Ext = x.File?.Ext,
                            Path = x.File?.Path
                        }).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }
            answerGroup.OtherAnswers = otherAnswers;
            return answerGroup;
        }

        private AnswerGroupDTO GetStepTaskAnswers(UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO();

            var currentUserStepTaskAnswers = currentUserStepResult.StepTaskAnswers.ToList();

            if (currentUserStepTaskAnswers != null)
            {
                answerGroup.Answer = new AnswerDTO
                {
                    StepTaskAnswers = currentUserStepTaskAnswers.Select(x => new StepTaskAnswerDTO
                    {
                        Id = x.Id,
                        Email = x.UserToPlanId != null ? x.UserToPlan.User.Email : x.Email,
                        FirstName = x.UserToPlanId != null ? x.UserToPlan.User.FirstName : x.FirstName,
                        LastName = x.UserToPlanId != null ? x.UserToPlan.User.LastName : x.LastName,
                        Step = x.StepTask.Step,
                        UserToPlanId = x.UserToPlanId
                    }).ToList()
                };
            }
            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveStepTaskAnswers = definitiveStepResult?.StepTaskAnswers.ToList();

            if (definitiveStepTaskAnswers != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    StepTaskAnswers = definitiveStepTaskAnswers.Select(x => new StepTaskAnswerDTO
                    {
                        Id = x.Id,
                        Email = x.UserToPlanId != null ? x.UserToPlan.User.Email : x.Email,
                        FirstName = x.UserToPlanId != null ? x.UserToPlan.User.FirstName : x.FirstName,
                        LastName = x.UserToPlanId != null ? x.UserToPlan.User.LastName : x.LastName,
                        Step = x.StepTask.Step,
                        UserToPlanId = x.UserToPlanId
                    }).ToList()
                };
            }

            var otherStepTaskAnswers = new List<AnswerDTO>();

            foreach (var otherStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var answerDTO = new AnswerDTO
                {
                    StepTaskAnswers = otherStepResult.StepTaskAnswers.Select(x => new StepTaskAnswerDTO
                    {
                        Id = x.Id,
                        Email = x.UserToPlanId != null ? x.UserToPlan.User.Email : x.Email,
                        FirstName = x.UserToPlanId != null ? x.UserToPlan.User.FirstName : x.FirstName,
                        LastName = x.UserToPlanId != null ? x.UserToPlan.User.LastName : x.LastName,
                        Step = x.StepTask.Step,
                        UserToPlanId = x.UserToPlanId
                    }).ToList(),

                    Author = $"{otherStepResult.UserToPlan.User.FirstName} {otherStepResult.UserToPlan.User.LastName}"
                };

                otherStepTaskAnswers.Add(answerDTO);
            }
            answerGroup.OtherAnswers = otherStepTaskAnswers;
            return answerGroup;
        }

        private AnswerGroupDTO GetValueAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.ValueAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    ValueAnswer = currentUserAnswer.Select(x => new ValueAnswerDTO
                    {
                        Id = x.Id,
                        Value = x.Value,
                        Definition = x.Definition,
                        Description = x.Description
                    }).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.ValueAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    ValueAnswer = definitiveAnswer.Select(x => new ValueAnswerDTO
                    {
                        Id = x.Id,
                        Value = x.Value,
                        Definition = x.Definition,
                        Description = x.Description
                    }).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.ValueAnswers.Where(x => x.QuestionId == questionId);

                if (userAnswer != null && userAnswer.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        ValueAnswer = userAnswer.Select(x => new ValueAnswerDTO
                        {
                            Id = x.Id,
                            Value = x.Value,
                            Definition = x.Definition,
                            Description = x.Description
                        }).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetSWOTAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.SWOTAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    SwotAnswer = new SWOTAnswerDTO
                    {
                        Strengths = currentUserAnswer.Where(x => x.Type == SWOTTypes.Strength).Select(x => x.Name).ToList(),
                        Weaknesses = currentUserAnswer.Where(x => x.Type == SWOTTypes.Weakness).Select(x => x.Name).ToList(),
                        Opportunities = currentUserAnswer.Where(x => x.Type == SWOTTypes.Opportunity).Select(x => x.Name).ToList(),
                        Threats = currentUserAnswer.Where(x => x.Type == SWOTTypes.Threat).Select(x => x.Name).ToList()
                    }
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.SWOTAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    SwotAnswer = new SWOTAnswerDTO
                    {
                        Strengths = definitiveAnswer.Where(x => x.Type == SWOTTypes.Strength).Select(x => x.Name).ToList(),
                        Weaknesses = definitiveAnswer.Where(x => x.Type == SWOTTypes.Weakness).Select(x => x.Name).ToList(),
                        Opportunities = definitiveAnswer.Where(x => x.Type == SWOTTypes.Opportunity).Select(x => x.Name).ToList(),
                        Threats = definitiveAnswer.Where(x => x.Type == SWOTTypes.Threat).Select(x => x.Name).ToList()
                    }
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.SWOTAnswers.Where(x => x.QuestionId == questionId);

                if (userAnswer != null && userAnswer.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        SwotAnswer = new SWOTAnswerDTO
                        {
                            Strengths = userAnswer.Where(x => x.Type == SWOTTypes.Strength).Select(x => x.Name).ToList(),
                            Weaknesses = userAnswer.Where(x => x.Type == SWOTTypes.Weakness).Select(x => x.Name).ToList(),
                            Opportunities = userAnswer.Where(x => x.Type == SWOTTypes.Opportunity).Select(x => x.Name).ToList(),
                            Threats = userAnswer.Where(x => x.Type == SWOTTypes.Threat).Select(x => x.Name).ToList()
                        },
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetStakeholderAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.StakeholderAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    StakeholderAnswers = currentUserAnswer.Select(x => new StakeholderAnswerDTO
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        IsInternal = x.IsInternal,
                        UserId = x.UserId,
                        CategoryId = x.CategoryId,
                        Category = x.Category?.Title
                    }).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.StakeholderAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    StakeholderAnswers = definitiveAnswer.Select(x => new StakeholderAnswerDTO
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        IsInternal = x.IsInternal,
                        UserId = x.UserId,
                        CategoryId = x.CategoryId,
                        Category = x.Category?.Title
                    }).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.StakeholderAnswers.Where(x => x.QuestionId == questionId);

                if (userAnswer != null && userAnswer.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        StakeholderAnswers = userAnswer.Select(x => new StakeholderAnswerDTO
                        {
                            Id = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Email = x.Email,
                            IsInternal = x.IsInternal,
                            UserId = x.UserId,
                            CategoryId = x.CategoryId,
                            Category = x.Category?.Title
                        }).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetStrategicIssueAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswers = currentUserStepResult.StrategicIssueAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswers.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    StrategicIssueAnswers = currentUserAnswers.Select(x => new StrategicIssueAnswerDTO
                    {
                        Goal = x.Goal,
                        Ranking = x.Ranking,
                        Result = x.Result,
                        Solution = x.Solution,
                        Why = x.Why,
                        IssueId = x.IssueId,
                        Issue = x.Issue.Name
                    }).OrderByDescending(x => x.Ranking).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswers = definitiveStepResult?.StrategicIssueAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswers != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    StrategicIssueAnswers = definitiveAnswers.Select(x => new StrategicIssueAnswerDTO
                    {
                        Goal = x.Goal,
                        Ranking = x.Ranking,
                        Result = x.Result,
                        Solution = x.Solution,
                        Why = x.Why,
                        IssueId = x.IssueId,
                        Issue = x.Issue.Name
                    }).OrderByDescending(x => x.Ranking).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswers = otherUserStepResult.StrategicIssueAnswers.Where(a => a.QuestionId == questionId);

                if (userAnswers != null)
                {
                    var answerDTO = new AnswerDTO
                    {
                        StrategicIssueAnswers = userAnswers.Select(x => new StrategicIssueAnswerDTO
                        {
                            Goal = x.Goal,
                            Ranking = x.Ranking,
                            Result = x.Result,
                            Solution = x.Solution,
                            Why = x.Why,
                            IssueId = x.IssueId,
                            Issue = x.Issue.Name
                        }).OrderByDescending(x => x.Ranking).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetStakeholdersRatingAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.StakeholderRatingAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    StakeholderRatingAnswers = currentUserAnswer.Select(x => new StakeholderRatingAnswerDTO
                    {
                        Grade = x.Grade,
                        Priority = x.Priority,
                        StakeholderId = x.StakeholderId,
                        StakeholderName = x.Stakeholder.FirstName + " " + x.Stakeholder.LastName,
                        CriterionsRates = x.Criteria.Select(y => new StakeholderRatingByCriterionDTO
                        {
                            CriterionId = y.CriterionId,
                            Rate = y.Rate
                        }).ToList()
                    }).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.StakeholderRatingAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    StakeholderRatingAnswers = definitiveAnswer.Select(x => new StakeholderRatingAnswerDTO
                    {
                        Grade = x.Grade,
                        Priority = x.Priority,
                        StakeholderId = x.StakeholderId,
                        StakeholderName = x.Stakeholder.FirstName + " " + x.Stakeholder.LastName,
                        CriterionsRates = x.Criteria.Select(y => new StakeholderRatingByCriterionDTO
                        {
                            CriterionId = y.CriterionId,
                            Rate = y.Rate
                        }).ToList()
                    }).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.StakeholderRatingAnswers.Where(x => x.QuestionId == questionId);

                if (userAnswer != null && userAnswer.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        StakeholderRatingAnswers = userAnswer.Select(x => new StakeholderRatingAnswerDTO
                        {
                            Grade = x.Grade,
                            Priority = x.Priority,
                            StakeholderId = x.StakeholderId,
                            StakeholderName = x.Stakeholder.FirstName + " " + x.Stakeholder.LastName,
                            CriterionsRates = x.Criteria.Select(y => new StakeholderRatingByCriterionDTO
                            {
                                CriterionId = y.CriterionId,
                                Rate = y.Rate
                            }).ToList()
                        }).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetIssueOptionsAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.IssueOptionAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    IssueOptionAnswers = currentUserAnswer.Select(x => new IssueOptionAnsweDTO
                    {
                        Id = x.Id,
                        IssueId = x.IssueId,
                        Actors = x.Actors,
                        IsBestOption = x.IsBestOption,
                        Option = x.Option,
                        Resources = String.Join(',', x.IssueOptionAnswersToResources.Select(y => y.Resource.Title).ToList())
                    }).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.IssueOptionAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    IssueOptionAnswers = definitiveAnswer.Select(x => new IssueOptionAnsweDTO
                    {
                        Id = x.Id,
                        IssueId = x.IssueId,
                        Actors = x.Actors,
                        IsBestOption = x.IsBestOption,
                        Option = x.Option,
                        Resources = String.Join(',', x.IssueOptionAnswersToResources.Select(y => y.Resource.Title).ToList())
                    }).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.IssueOptionAnswers.Where(x => x.QuestionId == questionId);

                if (userAnswer != null && userAnswer.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        IssueOptionAnswers = userAnswer.Select(x => new IssueOptionAnsweDTO
                        {
                            Id = x.Id,
                            IssueId = x.IssueId,
                            Actors = x.Actors,
                            IsBestOption = x.IsBestOption,
                            Option = x.Option,
                            Resources = String.Join(',', x.IssueOptionAnswersToResources.Select(y => y.Resource.Title).ToList())
                        }).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetIssueDistinguishAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId,
                Answer = new AnswerDTO()
                {
                    IssueDistinguishAnswers = new List<IssueDistinguishAnswerDTO>()
                },
                DefinitiveAnswer = new AnswerDTO()
                {
                    IssueDistinguishAnswers = new List<IssueDistinguishAnswerDTO>()
                },
                OtherAnswers = new List<AnswerDTO>()
            };

            var questions = GetIssueDistinguishQuestions();

            var userStepResultIssueDistinguishAnswers = new Dictionary<int, List<IssueDistinguishAnswerDTO>>();

            foreach (var question in questions)
            {
                if (question.Type == QuestionTypes.IssueDistinguishSelect || question.Type == QuestionTypes.IssueDistinguishTypeSelect)
                {
                    var selectAnswers = currentUserStepResult.SelectAnswers.Where(x => x.QuestionId == question.Id);

                    foreach (var selectAnswer in selectAnswers)
                    {
                        answerGroup.Answer.IssueDistinguishAnswers.Add(new IssueDistinguishAnswerDTO
                        {
                            QuestionId = question.Id,
                            IssueId = selectAnswer.IssueId ?? 0,
                            SelectAnswer = selectAnswer.OptionId ?? 0
                        });
                    }

                    var definitiveResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

                    if (definitiveResult != null)
                    {
                        var definitiveAnswers = definitiveResult.SelectAnswers.Where(x => x.QuestionId == question.Id);
                        foreach (var selectAnswer in definitiveAnswers)
                        {
                            answerGroup.DefinitiveAnswer.IssueDistinguishAnswers.Add(new IssueDistinguishAnswerDTO
                            {
                                QuestionId = question.Id,
                                IssueId = selectAnswer.IssueId ?? 0,
                                SelectAnswer = selectAnswer.OptionId ?? 0
                            });
                        }
                    }

                    foreach (var otherUserStepResult in otherUserStepResults)
                    {
                        var otherSelectAnswers = otherUserStepResult.SelectAnswers.Where(x => x.QuestionId == question.Id);

                        var issueDistinguishAnswers = otherSelectAnswers.Select(x => new IssueDistinguishAnswerDTO
                        {
                            QuestionId = question.Id,
                            IssueId = x.IssueId ?? 0,
                            SelectAnswer = x.OptionId ?? 0
                        }).ToList();

                        if (userStepResultIssueDistinguishAnswers.ContainsKey(otherUserStepResult.Id))
                        {
                            userStepResultIssueDistinguishAnswers[otherUserStepResult.Id].AddRange(issueDistinguishAnswers);
                        }
                        else
                        {
                            userStepResultIssueDistinguishAnswers.Add(otherUserStepResult.Id, issueDistinguishAnswers);
                        }
                    }
                }

                if (question.Type == QuestionTypes.IssueDistinguishMultiSelect)
                {
                    var selectAnswersGroups = currentUserStepResult.SelectAnswers.Where(x => x.QuestionId == question.Id).GroupBy(x => x.IssueId);

                    foreach (var selectAnswersGroup in selectAnswersGroups)
                    {
                        answerGroup.Answer.IssueDistinguishAnswers.Add(new IssueDistinguishAnswerDTO
                        {
                            QuestionId = question.Id,
                            IssueId = selectAnswersGroup.Key.Value,
                            SelectAnswers = selectAnswersGroup.Select(x => x.OptionId.Value).ToList()
                        });
                    }

                    var definitiveResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

                    if (definitiveResult != null)
                    {
                        var definitiveAnswerGroups = definitiveResult.SelectAnswers.Where(x => x.QuestionId == question.Id).GroupBy(x => x.IssueId);

                        foreach (var definitiveAnswerGroup in definitiveAnswerGroups)
                        {
                            answerGroup.DefinitiveAnswer.IssueDistinguishAnswers.Add(new IssueDistinguishAnswerDTO
                            {
                                QuestionId = question.Id,
                                IssueId = definitiveAnswerGroup.Key.Value,
                                SelectAnswers = definitiveAnswerGroup.Select(x => x.OptionId.Value).ToList()
                            });
                        }
                    }

                    foreach (var otherUserStepResult in otherUserStepResults)
                    {
                        var otherSelectAnswers = otherUserStepResult.SelectAnswers.Where(x => x.QuestionId == question.Id);

                        var issueDistinguishAnswers = otherSelectAnswers.GroupBy(x => x.IssueId).Select(x => new IssueDistinguishAnswerDTO
                        {
                            QuestionId = question.Id,
                            IssueId = x.Key ?? 0,
                            SelectAnswers = x.Select(k => k.OptionId.Value).ToList()
                        }).ToList();

                        if (userStepResultIssueDistinguishAnswers.ContainsKey(otherUserStepResult.Id))
                        {
                            userStepResultIssueDistinguishAnswers[otherUserStepResult.Id].AddRange(issueDistinguishAnswers);
                        }
                        else
                        {
                            userStepResultIssueDistinguishAnswers.Add(otherUserStepResult.Id, issueDistinguishAnswers);
                        }
                    }
                }
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var selectAnswers = otherUserStepResult.SelectAnswers.Where(x => questions.Any(s => s.Id == x.QuestionId));

                otherAnswers.Add(
                     new AnswerDTO
                     {
                         Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}",
                         IssueDistinguishAnswers = userStepResultIssueDistinguishAnswers[otherUserStepResult.Id]
                     });
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        private AnswerGroupDTO GetResourceReviewAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswers = currentUserStepResult.BooleanAnswers.Where(x => x.QuestionId == questionId);

            if (currentUserAnswers.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    ResourceReviewAnswers = currentUserAnswers.Select(x => new ResourceReviewAnswerDTO
                    {
                        Assured = x.Answer,
                        ResourceId = x.ResourceId.Value
                    }).ToList()
                };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswers = definitiveStepResult?.BooleanAnswers.Where(x => x.QuestionId == questionId);

            if (definitiveAnswers != null && definitiveAnswers.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    ResourceReviewAnswers = currentUserAnswers.Select(x => new ResourceReviewAnswerDTO
                    {
                        Assured = x.Answer,
                        ResourceId = x.ResourceId.Value
                    }).ToList()
                };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswers = otherUserStepResult.BooleanAnswers.Where(a => a.QuestionId == questionId);

                if (userAnswers.Any())
                {
                    var answerDTO = new AnswerDTO
                    {
                        ResourceReviewAnswers = userAnswers.Select(x => new ResourceReviewAnswerDTO
                        {
                            Assured = x.Answer,
                            ResourceId = x.ResourceId.Value
                        }).ToList(),
                        Author = $"{otherUserStepResult.UserToPlan.User.FirstName} {otherUserStepResult.UserToPlan.User.LastName}"
                    };

                    otherAnswers.Add(answerDTO);
                }
            }

            answerGroup.OtherAnswers = otherAnswers;

            return answerGroup;
        }

        #endregion

        #endregion
    }
}
