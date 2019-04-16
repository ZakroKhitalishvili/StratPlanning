using Core.Context;
using Core.Entities;
using System;
using Application.Interfaces.Repositories;
using Application.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories
{
    public class PlanRepository : RepositoryBase<Plan>, IPlanRepository
    {
        public PlanRepository(PlanningDbContext context) : base(context)
        { }

        public void CreatePlan(Plan plan)
        {
            throw new NotImplementedException();
        }

        public PlanStepDTO GetStep(string stepIndex)
        {
            var blocks = Context.StepBlocks.Where(x => x.Step == stepIndex)
                .Include(x => x.Questions)
                .OrderBy(x => x.Order)
                .Select(x => new StepBlockDTO
                {
                    Title = x.Title,
                    Description = x.Description,
                    Instruction = x.Instruction,
                    Order = x.Order,
                    Questions = x.Questions.Select(q => new QuestionDTO
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Description = q.Description,
                        Type = q.Type,
                        Options = q.HasOptions ? q.Options.Select(o => new OptionDTO()
                        {
                            Id = o.Id,
                            Title = o.Title,
                            Description = o.Description
                        }).ToList() : null
                    }).ToList()

                }).ToList();

            var stepDTO = new PlanStepDTO
            {
                Step = stepIndex,
                StepBlocks = blocks
            };

            return stepDTO;

        }

    }
}
