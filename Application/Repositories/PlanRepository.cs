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

        public void CreatePlan(Plan plan)
        {
            throw new NotImplementedException();
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

        public PlanStepDTO GetStep(string stepIndex, int planId, int userId)
        {
            var blocksDTOs = GetStepStructure(stepIndex);

            var userToPlans = Context.UsersToPlans
                .Where(x => x.PlanId == planId)
                .Include(x => x.User)
                .ToList();

            var currentUserToPlan = userToPlans.Where(x => x.UserId == userId).FirstOrDefault();

            FillWithAnswers(blocksDTOs, currentUserToPlan, userToPlans, stepIndex);

            var stepDTO = new PlanStepDTO
            {
                PlanId = planId,
                Step = stepIndex,
                StepBlocks = blocksDTOs.ToList(),
                PlanningTeam = GetPlanningTeam(planId)
            };

            return stepDTO;
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
            var stepUserResult = GetUserStepResult(planStep.PlanId, planStep.Step, isDefinitive, userId);

            if (!stepUserResult.IsFinal && stepUserResult.IsSubmitted)
            {
                return false;
            }

            stepUserResult.IsSubmitted = isSubmitted;

            SaveStepBlocks(planStep.StepBlocks, stepUserResult);

            Context.SaveChanges();

            return true;

        }

        #region Private methods

        private UserStepResult GetUserStepResult(int planId, string stepIndex, bool isDefinitive, int userId)
        {
            UserStepResult userStepResult;

            if (isDefinitive)
            {
                userStepResult = Context.UserStepResults
                    .Where(x => x.Step == stepIndex && x.PlanId == planId)
                    .Include(x => x.BooleanAnswers)
                    .FirstOrDefault();
            }
            else
            {
                userStepResult = Context.UserStepResults
                    .Where(x => x.Step == stepIndex)
                    .Include(x => x.UserToPlan)
                    .Where(x => x.UserToPlan.UserId == userId && x.UserToPlan.PlanId == planId)
                    .Include(x => x.BooleanAnswers)
                    .FirstOrDefault();
            }

            if (userStepResult == null)
            {
                userStepResult = new UserStepResult
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    IsFinal = isDefinitive,
                    PlanId = planId,
                    Step = stepIndex
                };

                if (!isDefinitive)
                {
                    userStepResult.PlanId = null;
                    var userToPlan = Context.UsersToPlans.Where(x => x.UserId == userId && x.PlanId == planId).FirstOrDefault();
                    userStepResult.UserToPlan = userToPlan;
                }

                Context.UserStepResults.Add(userStepResult);
                Context.SaveChanges();
            }

            return userStepResult;

        }

        private void SaveStepBlocks(IList<StepBlockDTO> stepBlocks, UserStepResult userStepResult)
        {
            foreach (var stepBlock in stepBlocks)
            {
                foreach (var question in stepBlock.Questions)
                {
                    if (question.Type == QuestionTypes.Boolean)
                    {
                        SaveBooleanAnswer(question, userStepResult);
                    }
                }
            }
        }

        private void SaveBooleanAnswer(QuestionDTO question, UserStepResult userStepResult)
        {
            BooleanAnswer dbAnswer = null;

            if (userStepResult.BooleanAnswers != null)
            {
                dbAnswer = userStepResult.BooleanAnswers.Where(x => x.QuestionId == question.Id).FirstOrDefault();
            }
            else
            {
                userStepResult.BooleanAnswers = new HashSet<BooleanAnswer>();
            }

            if (dbAnswer != null)
            {
                dbAnswer.Answer = question.Answer.BooleanAnswer.Answer;
                dbAnswer.UpdatedAt = DateTime.Now;
                dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
            }
            else
            {
                var newAnswer = new BooleanAnswer
                {
                    Answer = question.Answer.BooleanAnswer.Answer,
                    QuestionId = question.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userStepResult.UpdatedBy,
                    UpdatedBy = userStepResult.UpdatedBy
                };

                userStepResult.BooleanAnswers.Add(newAnswer);
            }

        }

        private IList<StepBlockDTO> GetStepStructure(string stepIndex)
        {
            return Context.StepBlocks.Where(x => x.Step == stepIndex)
                .Include(x => x.Questions)
                .ThenInclude(x => x.Options)
                .OrderBy(x => x.Order)
                .AsEnumerable()
                .Select(x => Mapper.Map<StepBlockDTO>(x))
                .ToList();
        }


        private void FillWithAnswers(IList<StepBlockDTO> stepBlocks, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex)
        {

            for (int i = 0; i < stepBlocks.Count; i++)
            {
                var questions = stepBlocks[i].Questions;
                for (int j = 0; j < questions.Count; j++)
                {
                    if (questions[j].Type == QuestionTypes.Boolean)
                    {
                        FillBooleanQuestion(questions[j], currentUserToPlan, userToPlans, stepIndex);
                    }
                }
            }
        }

        private void FillBooleanQuestion(QuestionDTO question, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex)
        {
            GetAnswers<BooleanAnswer>(currentUserToPlan,
                userToPlans,
                stepIndex,
                out BooleanAnswer currentUserAnswer,
                out IList<BooleanAnswer> otherAnswers,
                out BooleanAnswer definitiveAnswer);

            if (currentUserAnswer != null)
            {
                question.Answer = new AnswerDTO { BooleanAnswer = new BooleanAnswerDTO { Answer = currentUserAnswer.Answer } };
            }

            if (definitiveAnswer != null)
            {
                question.DefinitiveAnswer = new AnswerDTO { BooleanAnswer = new BooleanAnswerDTO { Answer = definitiveAnswer.Answer } };
            }

            question.OtherAnswers = otherAnswers.Select(x => new AnswerDTO
            {
                BooleanAnswer = new BooleanAnswerDTO
                {
                    Answer = x.Answer,
                },
                Author = $"{x.UserStepResult.UserToPlan.User.FirstName} {x.UserStepResult.UserToPlan.User.LastName}"
            });
        }

        private void GetAnswers<T>(UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex, out T currentUserAnswer, out IList<T> otherAnswers, out T definitiveAnswer) where T : AbstractAnswer
        {
            var currentUserStepResult = Context.UserStepResults
                .Where(x => x.UserToPlanId == currentUserToPlan.Id && x.Step == stepIndex)
                .FirstOrDefault();

            currentUserAnswer = null;
            if (currentUserStepResult != null)
            {
                currentUserAnswer = Context.Set<T>().Where(x => x.UserStepResultId == currentUserStepResult.Id).FirstOrDefault();
            }

            otherAnswers = new List<T>();

            foreach (var userToPlan in userToPlans)
            {
                if (userToPlan.Id != currentUserToPlan.Id)
                {
                    var userStepResult = Context.UserStepResults
                        .Where(x => x.UserToPlanId == userToPlan.Id && x.Step == stepIndex)
                        .FirstOrDefault();

                    T userAnswer;
                    if (userStepResult != null)
                    {
                        userAnswer = Context.Set<T>().Where(x => x.UserStepResultId == userStepResult.Id)
                            .Include(x => x.UserStepResult)
                            .ThenInclude(x => x.UserToPlan)
                            .ThenInclude(x => x.User)
                            .FirstOrDefault();

                        otherAnswers.Add(userAnswer);
                    }

                }
            }

            var adminResult = Context.UserStepResults
                .Where(x => x.IsFinal && x.PlanId == currentUserToPlan.PlanId && x.Step == stepIndex)
                .FirstOrDefault();

            definitiveAnswer = null;
            if (currentUserStepResult != null)
            {
                definitiveAnswer = Context.Set<T>().Where(x => x.UserStepResultId == adminResult.Id).FirstOrDefault();
            }

        }

        #endregion
    }
}
