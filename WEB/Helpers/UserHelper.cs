using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Helpers
{
    public class UserHelper
    {
        public static IEnumerable<SelectListItem> GetUsersSelectList(HttpContext context)
        {
            var userRepository = context.RequestServices.GetService<IUserRepository>();

            var selectList = userRepository.FindAll().Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.FirstName} {u.LastName}" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> GetUsersSelectListByRole(HttpContext context, string role)
        {
            var userRepository = context.RequestServices.GetService<IUserRepository>();

            var selectList = userRepository.FindByCondition(x => x.Role == role).Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.FirstName} {u.LastName}" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> GetUsersSelectListByPlan(HttpContext context, int planId)
        {
            var planRepository = context.RequestServices.GetService<IPlanRepository>();

            var selectList = planRepository.GetPlanningTeam(planId).Select(u => new SelectListItem { Value = u.UserToPlanId.ToString(), Text = u.FullName });

            return selectList;
        }

        public static IEnumerable<SelectListItem> GetPositionsSelectList(HttpContext context)
        {
            var dictionaryRepository = context.RequestServices.GetService<IDictionaryRepository>();

            var selectList = dictionaryRepository.GetPositions().Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title });

            return selectList;
        }

        public static IEnumerable<SelectListItem> GetCategoriesSelectList(HttpContext context)
        {
            var dictionaryRepository = context.RequestServices.GetService<IDictionaryRepository>();

            var selectList = dictionaryRepository.GetStakeholderCategories().Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title });

            return selectList;
        }
    }
}
