using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Helpers
{
    public class Stephelper
    {

        public static IEnumerable<PlanStepDTO> GetStepList(HttpContext context)
        {
            var planRepository = context.RequestServices.GetService<IPlanRepository>();
            return planRepository.GetStepList();
        }
    }
}
