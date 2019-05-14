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

            var planStep = new PlanStepDTO
            {
                PlanId = planId,
                Step = stepIndex,
                StepBlocks = blocksDTOs.ToList(),
                PlanningTeam = GetPlanningTeam(planId)
            };

            FillWithAnswers(planStep, currentUserToPlan, userToPlans);

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
            var stepUserResult = GetUserStepResult(planStep.PlanId, planStep.Step, isDefinitive, userId);

            if (!stepUserResult.IsDefinitive && stepUserResult.IsSubmitted)
            {
                return false;
            }

            stepUserResult.IsSubmitted = isSubmitted;

            SaveAnswers(planStep.AnswerGroups, stepUserResult);

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
                    .Include(x => x.SelectAnswers).ThenInclude(x => x.Option)
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
                    IsDefinitive = isDefinitive,
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

            }
        }

        private void SaveBooleanAnswer(AnswerGroupDTO answerGroup, UserStepResult userStepResult)
        {
            BooleanAnswer dbAnswer = null;

            dbAnswer = userStepResult.BooleanAnswers.Where(x => x.QuestionId == answerGroup.QuestionId).FirstOrDefault();

            if (dbAnswer != null)
            {
                dbAnswer.Answer = answerGroup.Answer.BooleanAnswer;
                dbAnswer.UpdatedAt = DateTime.Now;
                dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
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

            if (dbAnswer != null)
            {
                dbAnswer.OptionId = answerGroup.Answer.SelectAnswer.OptionId;
                dbAnswer.AltOption = answerGroup.Answer.SelectAnswer.OptionId == null ? answerGroup.Answer.SelectAnswer.AltOption : null;
                dbAnswer.UpdatedAt = DateTime.Now;
                dbAnswer.UpdatedBy = userStepResult.UpdatedBy;
            }
            else
            {
                var newAnswer = new SelectAnswer
                {
                    OptionId = answerGroup.Answer.SelectAnswer.OptionId,
                    AltOption = answerGroup.Answer.SelectAnswer.OptionId == null ? answerGroup.Answer.SelectAnswer.AltOption : null,
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


        private void FillWithAnswers(PlanStepDTO planStep, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans)
        {
            planStep.AnswerGroups = new List<AnswerGroupDTO>();

            for (int i = 0; i < planStep.StepBlocks.Count; i++)
            {
                var questions = planStep.StepBlocks[i].Questions;

                for (int j = 0; j < questions.Count; j++)
                {
                    if (questions[j].Type == QuestionTypes.Boolean)
                    {
                        planStep.AnswerGroups.Add(GetBooleanAnswers(questions[j].Id, currentUserToPlan, userToPlans, planStep.Step));
                    }

                    if (questions[j].Type == QuestionTypes.Select)
                    {
                        planStep.AnswerGroups.Add(GetSelectAnswers(questions[j].Id, currentUserToPlan, userToPlans, planStep.Step));
                    }

                    if (questions[j].Type == QuestionTypes.TagMultiSelect)
                    {
                        planStep.AnswerGroups.Add(GetTagMultiSelectAnswers(questions[j].Id, currentUserToPlan, userToPlans, planStep.Step));
                    }

                    if (questions[j].Type == QuestionTypes.PlanTypeSelect)
                    {
                        planStep.AnswerGroups.Add(GetBooleanAnswers(questions[j].Id, currentUserToPlan, userToPlans, planStep.Step));
                    }

                }
            }
        }

        private AnswerGroupDTO GetBooleanAnswers(int questionId, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex)
        {
            GetAnswers<BooleanAnswer>(questionId,
                currentUserToPlan,
                userToPlans,
                stepIndex,
                out IList<BooleanAnswer> currentUserAnswer,
                out IList<BooleanAnswer> otherAnswers,
                out IList<BooleanAnswer> definitiveAnswer);

            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO { BooleanAnswer = currentUserAnswer.First().Answer };
            }

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO { BooleanAnswer = definitiveAnswer.First().Answer };
            }

            answerGroup.OtherAnswers = otherAnswers.Select(x => new AnswerDTO
            {
                BooleanAnswer = x.Answer,
                Author = $"{x.UserStepResult.UserToPlan.User.FirstName} {x.UserStepResult.UserToPlan.User.LastName}"
            });

            return answerGroup;
        }

        private AnswerGroupDTO GetPlanTypeSelectAnswers(int questionId, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex)
        {
            GetAnswers<BooleanAnswer>(questionId,
                currentUserToPlan,
                userToPlans,
                stepIndex,
                out IList<BooleanAnswer> currentUserAnswer,
                out IList<BooleanAnswer> otherAnswers,
                out IList<BooleanAnswer> definitiveAnswer);

            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO { BooleanAnswer = currentUserAnswer.First().Answer };
            }

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO { BooleanAnswer = definitiveAnswer.First().Answer };
            }

            answerGroup.OtherAnswers = otherAnswers.Select(x => new AnswerDTO
            {
                BooleanAnswer = x.Answer,
                Author = $"{x.UserStepResult.UserToPlan.User.FirstName} {x.UserStepResult.UserToPlan.User.LastName}"
            });

            return answerGroup;
        }


        private AnswerGroupDTO GetSelectAnswers(int questionId, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex)
        {
            GetAnswers<SelectAnswer>(questionId,
                currentUserToPlan,
                userToPlans,
                stepIndex,
                out IList<SelectAnswer> currentUserAnswer,
                out IList<SelectAnswer> otherAnswers,
                out IList<SelectAnswer> definitiveAnswer);

            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                answerGroup.Answer = new AnswerDTO
                {
                    SelectAnswer = new SelectAnswerDTO
                    {
                        OptionId = currentUserAnswer.First().OptionId,
                        AltOption = currentUserAnswer.First().AltOption
                    }
                };
            }

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                answerGroup.DefinitiveAnswer = new AnswerDTO
                {
                    SelectAnswer = new SelectAnswerDTO
                    {
                        OptionId = definitiveAnswer.First().OptionId,
                        AltOption = definitiveAnswer.First().AltOption
                    }
                };
            }

            answerGroup.OtherAnswers = otherAnswers.Select(x => new AnswerDTO
            {
                SelectAnswer = new SelectAnswerDTO
                {
                    OptionId = x.OptionId,
                    AltOption = x.AltOption
                },

                Author = $"{x.UserStepResult.UserToPlan.User.FirstName} {x.UserStepResult.UserToPlan.User.LastName}"
            });

            return answerGroup;
        }

        private AnswerGroupDTO GetTagMultiSelectAnswers(int questionId, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex)
        {
            GetAnswers<SelectAnswer>(questionId,
                currentUserToPlan,
                userToPlans,
                stepIndex,
                out IList<SelectAnswer> currentUserAnswer,
                out IList<SelectAnswer> otherAnswers,
                out IList<SelectAnswer> definitiveAnswer);

            AnswerGroupDTO answerGroup = new AnswerGroupDTO
            {
                QuestionId = questionId
            };

            if (currentUserAnswer != null && currentUserAnswer.Any())
            {
                foreach (var answer in currentUserAnswer)
                {
                    Context.Entry(answer).Reference(x => x.Option).Load();
                }

                answerGroup.Answer = new AnswerDTO
                {
                    TagSelectAnswers = currentUserAnswer.Select(x => x.OptionId != null ? x.Option.Title : x.AltOption).ToList()
                };
            }

            if (definitiveAnswer != null && definitiveAnswer.Any())
            {
                foreach (var answer in definitiveAnswer)
                {

                    Context.Entry(answer).Reference(x => x.Option).Load();
                }

                answerGroup.Answer = new AnswerDTO
                {
                    TagSelectAnswers = definitiveAnswer.Select(x => x.OptionId != null ? x.Option.Title : x.AltOption).ToList()
                };
            }

            foreach (var answer in otherAnswers)
            {
                Context.Entry(answer).Reference(x => x.Option).Load();
            }

            answerGroup.OtherAnswers = otherAnswers.GroupBy(x => x.UserStepResultId).Select(x => new AnswerDTO
            {
                TagSelectAnswers = x.Select(s => s.OptionId != null ? s.Option.Title : s.AltOption).ToList(),

                Author = $"{x.First().UserStepResult.UserToPlan.User.FirstName} {x.First().UserStepResult.UserToPlan.User.LastName}"
            });

            return answerGroup;
        }

        private void GetAnswers<T>(int questionId, UserToPlan currentUserToPlan, IList<UserToPlan> userToPlans, string stepIndex, out IList<T> currentUserAnswers, out IList<T> otherAnswers, out IList<T> definitiveAnswer) where T : AbstractAnswer
        {
            var currentUserStepResult = Context.UserStepResults
                .Where(x => x.UserToPlanId == currentUserToPlan.Id && x.Step == stepIndex)
                .FirstOrDefault();

            currentUserAnswers = null;
            if (currentUserStepResult != null)
            {
                currentUserAnswers = Context.Set<T>().Where(x => x.UserStepResultId == currentUserStepResult.Id && x.QuestionId == questionId).ToList();
            }

            otherAnswers = new List<T>();

            foreach (var userToPlan in userToPlans)
            {
                if (userToPlan.Id != currentUserToPlan.Id)
                {
                    var userStepResult = Context.UserStepResults
                        .Where(x => x.UserToPlanId == userToPlan.Id && x.Step == stepIndex)
                        .FirstOrDefault();

                    List<T> userAnswers;
                    if (userStepResult != null)
                    {
                        userAnswers = Context.Set<T>().Where(x => x.UserStepResultId == userStepResult.Id && x.QuestionId == questionId)
                            .Include(x => x.UserStepResult)
                            .ThenInclude(x => x.UserToPlan)
                            .ThenInclude(x => x.User)
                            .ToList();

                        foreach (var userAnswer in userAnswers)
                        {
                            otherAnswers.Add(userAnswer);
                        }

                    }

                }
            }

            var adminResult = Context.UserStepResults
                .Where(x => x.IsDefinitive && x.PlanId == currentUserToPlan.PlanId && x.Step == stepIndex)
                .FirstOrDefault();

            definitiveAnswer = null;
            if (adminResult != null)
            {
                definitiveAnswer = Context.Set<T>().Where(x => x.UserStepResultId == adminResult.Id).ToList();
            }

        }

        #endregion
    }
}
