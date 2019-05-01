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

        public static IEnumerable<SelectListItem> GetPositionsSelectList(HttpContext context)
        {
            var dictionaryRepository = context.RequestServices.GetService<IDictionaryRepository>();

            var selectList = dictionaryRepository.GetPositions().Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title });

            return selectList;
        }
    }
}
