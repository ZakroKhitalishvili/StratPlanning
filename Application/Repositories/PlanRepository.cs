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

namespace Application.Repositories
{
    public class PlanRepository : RepositoryBase<Plan>, IPlanRepository
    {
        public PlanRepository(PlanningDbContext context) : base(context)
        { }

        public bool AddUserToPlan(int userId, int planId, int positionId)
        {
            if (Context.UsersToPlans.Where(x => x.UserId == userId && x.PlanId == planId).Any())
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

                GetOrCreateStepTask(newPlan.Id, Steps.Predeparture, userId);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<PlanDTO> GetPlanList()
        {
            return FindAll().Select(p => Mapper.Map<PlanDTO>(p));
        }

        public IEnumerable<UserPlanningMemberDTO> GetPlanningTeam(int planId)
        {
            return Context.UsersToPlans
                .Where(x => x.PlanId == planId).Include(x => x.User).Include(x => x.Position)
                .AsEnumerable()
                .Select(x => new UserPlanningMemberDTO { Id = x.User.Id, FullName = $"{x.User.FirstName} {x.User.LastName}", Position = x.Position?.Title })
                .ToList();
        }

        public PlanStepDTO GetStep(string stepIndex, int planId, bool isDefinitive, int userId)
        {
            // checks if a requested step is active in plan's tasks or just planId or/and stepIndex are wrong
            //var stepTask = GetStepTask(planId, stepIndex);

            //if (stepTask == null)
            //{
            //    return null;
            //}
            //

            var planStep = GetPlanStep(planId, stepIndex, isDefinitive);

            var otherUserStepResults = GetPlanStepResults(planId, stepIndex).Where(x => x.IsSubmitted).ToList();

            UserStepResult currentUserStepResult = GetOrCreateUserStepResult(planId, stepIndex, isDefinitive, userId);

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

        public IEnumerable<PlanStepDTO> GetStepList()
        {
            var members = typeof(Steps).GetMembers().Where(x => x.MemberType == System.Reflection.MemberTypes.Field);
            return members.Select(member => new PlanStepDTO { Step = member.Name });
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
            //var stepTask = GetStepTask(planStep.PlanId, planStep.Step);

            //if (stepTask == null)
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
                Context.SaveChanges();
            }
            else
            {
                /*
                 * 1. save - nothing result -> create final aris
                 * 2. save - have one result thas is not submitted-> save in the this rezult, and this is final aris
                 * 3. save - have one result that is submitted -> create new final result, make submitted one not final aris
                 * 4. save - have two results -> save in final aris
                 * 5. submit - nothing -> create final and submitted aris
                 * 6. submit - have one result that is not submitted -> save in the result and submit aris
                 * 7. submit - have one result that is submitted -> save in the result aris
                 * 8. submit - have two results -> submit final one and unsubmit submitted one aris
                 * 
                 */
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

                var otherDefinitiveResult = Context.UserStepResults.Where(x => x.IsFinal.HasValue && !x.IsFinal.Value && !x.IsSubmitted && x.IsDefinitive).SingleOrDefault();

                if (otherDefinitiveResult != null)
                {
                    DeleteUserStepResult(otherDefinitiveResult.Id);
                }

                SaveAnswers(planStep.AnswerGroups, finalDefinitiveStepResult);

                Context.SaveChanges();
            }
            return true;
        }

        public IEnumerable<PlanDTO> GetPlanListForUser(int userId)
        {
            return Context.UsersToPlans
                .Where(x => x.UserId == userId)
                .Include(x => x.Plan)
                .Select(x => Mapper.Map<PlanDTO>(x.Plan)).ToList();
        }

        #region Private methods

        #region General methods

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

        private UserStepResult GetUserStepResult(int planId, string stepIndex, int userId)
        {
            return Context.UserStepResults
                    .Where(x => x.Step == stepIndex)
                    .Include(x => x.UserToPlan).ThenInclude(x => x.User)
                    .Where(x => x.UserToPlan.UserId == userId && x.UserToPlan.PlanId == planId)
                    .Include(x => x.BooleanAnswers)
                    .Include(x => x.SelectAnswers).ThenInclude(x => x.Option)
                    .Include(x => x.TextAnswers)
                    .SingleOrDefault();
        }

        private UserStepResult GetSubmittedDefinitiveStepResult(int planId, string stepIndex)
        {
            return Context.UserStepResults
                    .Where(x => x.Step == stepIndex && x.PlanId == planId && x.IsDefinitive && x.IsSubmitted)
                    .Include(x => x.BooleanAnswers)
                    .Include(x => x.SelectAnswers).ThenInclude(x => x.Option)
                    .Include(x => x.TextAnswers)
                    .SingleOrDefault();
        }

        private UserStepResult GetFinalDefinitiveStepResult(int planId, string stepIndex)
        {
            return Context.UserStepResults
                    .Where(x => x.Step == stepIndex && x.PlanId == planId && x.IsDefinitive && x.IsFinal.HasValue && x.IsFinal.Value)
                    .Include(x => x.BooleanAnswers)
                    .Include(x => x.SelectAnswers).ThenInclude(x => x.Option)
                    .Include(x => x.TextAnswers)
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
            var userStepResult = Context.UserStepResults.Where(x => x.Id == id)
                .Include(x => x.BooleanAnswers).Include(x => x.SelectAnswers).SingleOrDefault();

            if (userStepResult != null)
            {

                Context.BooleanAnswers.RemoveRange(userStepResult.BooleanAnswers);
                Context.SelectAnswers.RemoveRange(userStepResult.SelectAnswers);
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
                IsCompleted = steptask?.IsCompleted ?? false
            };

            if (isDefinitive)
            {
                var involvedUsers = Context.UsersToPlans
                    .Where(x => x.PlanId == planId)
                    .Include(x => x.User);

                var userStepResults = Context.UserStepResults.Where(x => x.PlanId == planId && x.Step == stepIndex && !x.IsDefinitive).ToList();

                planStep.SubmittedUsers = involvedUsers.Where(involvedUser => userStepResults.Where(x => x.UserToPlanId == involvedUser.Id && x.IsSubmitted).Any())
                    .Select(x => new UserPlanningMemberDTO
                    {
                        FullName = $"{x.User.FirstName} {x.User.LastName}"
                    });

                planStep.NotSubmittedUsers = involvedUsers.Where(involvedUser => !userStepResults.Where(x => x.UserToPlanId == involvedUser.Id && x.IsSubmitted).Any())
                   .Select(x => new UserPlanningMemberDTO
                   {
                       FullName = $"{x.User.FirstName} {x.User.LastName}"
                   });
            }

            return planStep;
        }

        #endregion

        #region Saving methods

        private void SaveAnswers(IList<AnswerGroupDTO> answerGroups, UserStepResult userStepResult)
        {
            foreach (var answerGroup in answerGroups)
            {
                var question = Context.Questions.Where(x => x.Id == answerGroup.QuestionId).FirstOrDefault();

                if (question.Type == QuestionTypes.Boolean)
                {
                    SaveBooleanAnswer(answerGroup, userStepResult);
                }

                if (question.Type == QuestionTypes.Select)
                {
                    SaveSelectAnswer(answerGroup, userStepResult);
                }

                if (question.Type == QuestionTypes.TagMultiSelect)
                {
                    SaveTagMultiSelectAnswer(answerGroup, userStepResult);
                }
                if (question.Type == QuestionTypes.PlanTypeSelect)
                {
                    SaveBooleanAnswer(answerGroup, userStepResult);
                }

                if (question.Type == QuestionTypes.TextArea)
                {
                    SaveTextAnswer(answerGroup, userStepResult);
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
                    dbAnswer.Answer = answerGroup.Answer.BooleanAnswer;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }
            else
            {
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
                    dbAnswer.OptionId = answerGroup.Answer.SelectAnswer.OptionId;
                    dbAnswer.AltOption = answerGroup.Answer.SelectAnswer.AltOption;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }
            else
            {
                var newAnswer = new SelectAnswer
                {
                    OptionId = answerGroup.Answer.SelectAnswer.OptionId,
                    AltOption = answerGroup.Answer.SelectAnswer.AltOption,
                    QuestionId = answerGroup.QuestionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userStepResult.UpdatedBy,
                    UpdatedBy = userStepResult.UpdatedBy
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

                if (!answerGroup.Answer.TagSelectAnswers.Contains(answerText))
                {
                    Context.SelectAnswers.Remove(dbAnswer);
                    Context.SaveChanges();
                    //userStepResult.SelectAnswers.Remove(dbAnswer);
                }
            }

            foreach (var answer in answerGroup.Answer.TagSelectAnswers)
            {
                if (!dbAnswers.Any(x => x.Option?.Title == answer || x.AltOption == answer))
                {
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

        private void SaveTextAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            TextAnswer dbAnswer = null;

            dbAnswer = userStepResult.TextAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).FirstOrDefault();

            if (dbAnswer != null)
            {
                if (!dbAnswer.Text.Equals(answerGroup.Answer.TextAnswer.Text))
                {
                    dbAnswer.Text = answerGroup.Answer.TextAnswer.Text;
                    dbAnswer.UpdatedAt = DateTime.Now;
                    dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
                }
            }
            else
            {
                var newAnswer = new TextAnswer
                {
                    Text = answerGroup.Answer.TextAnswer.Text,
                    QuestionId = answerGroup.QuestionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userStepResult.UpdatedBy,
                    UpdatedBy = userStepResult.UpdatedBy
                };

                userStepResult.TextAnswers.Add(newAnswer);
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
                    if (questions[j].Type == QuestionTypes.Boolean)
                    {
                        planStep.AnswerGroups.Add(GetBooleanAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults));
                    }

                    if (questions[j].Type == QuestionTypes.Select)
                    {
                        planStep.AnswerGroups.Add(GetSelectAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults));
                    }

                    if (questions[j].Type == QuestionTypes.TagMultiSelect)
                    {
                        planStep.AnswerGroups.Add(GetTagMultiSelectAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults));
                    }

                    if (questions[j].Type == QuestionTypes.PlanTypeSelect)
                    {
                        planStep.AnswerGroups.Add(GetBooleanAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults));
                    }
                    if (questions[j].Type == QuestionTypes.TextArea)
                    {
                        planStep.AnswerGroups.Add(GetTextAnswers(questions[j].Id, currentUserStepResult, otherUserStepResults));
                    }
                }
            }
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
                        AltOption = currentUserAnswer.AltOption
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
                        AltOption = definitiveAnswer.AltOption
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
                            AltOption = userAnswer.AltOption
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

        private AnswerGroupDTO GetTextAnswers(int questionId, UserStepResult currentUserStepResult, IList<UserStepResult> otherUserStepResults)
        {
            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            var currentUserAnswer = currentUserStepResult.TextAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (currentUserAnswer != null)
            {
                answerGroup.Answer = new AnswerDTO { TextAnswer = new TextAnswerDTO { IsIssue = currentUserAnswer.IsIssue, IsStakeholder = currentUserAnswer.IsStakeholder, Text = currentUserAnswer.Text } };
            }

            var definitiveStepResult = otherUserStepResults.Where(x => x.IsDefinitive).SingleOrDefault();

            var definitiveAnswer = definitiveStepResult?.TextAnswers.Where(x => x.QuestionId == questionId).SingleOrDefault();

            if (definitiveAnswer != null)
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO { TextAnswer = new TextAnswerDTO { IsIssue = definitiveAnswer.IsIssue, IsStakeholder = definitiveAnswer.IsStakeholder, Text = definitiveAnswer.Text } };
            }

            var otherAnswers = new List<AnswerDTO>();

            foreach (var otherUserStepResult in otherUserStepResults.Where(x => !x.IsDefinitive))
            {
                var userAnswer = otherUserStepResult.TextAnswers.Where(a => a.QuestionId == questionId).SingleOrDefault();

                if (userAnswer != null)
                {
                    var answerDTO = new AnswerDTO
                    {
                        TextAnswer = new TextAnswerDTO { IsIssue = userAnswer.IsIssue, IsStakeholder = userAnswer.IsStakeholder, Text = userAnswer.Text },
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
