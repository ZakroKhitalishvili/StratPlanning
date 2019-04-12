using Contracts.Repositories;
using Core.Context;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
