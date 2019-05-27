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
    public class StepHelper
    {

        public static IEnumerable<string> GetStepList(HttpContext context)
        {
            var planRepository = context.RequestServices.GetService<IPlanRepository>();
            return planRepository.GetStepList();
        }

        public static IEnumerable<string> GetValuesDictionary(HttpContext context)
        {
            var dictionaryRepository = context.RequestServices.GetService<IDictionaryRepository>();
            return dictionaryRepository.GetValues();
        }

        public static IEnumerable<IssueDTO> GetIssues(HttpContext context, int planId)
        {
            var planRepository = context.RequestServices.GetService<IPlanRepository>();
            return planRepository.GetIssues(planId);
        }
    }
}
