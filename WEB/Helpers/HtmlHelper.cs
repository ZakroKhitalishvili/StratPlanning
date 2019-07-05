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
    /// <summary>
    /// Helper class for getting data in Razor objects
    /// </summary>
    public class HtmlHelper
    {
        private readonly IUserRepository _userRepository;

        private readonly IPlanRepository _planRepository;

        private readonly IDictionaryRepository _dictionaryRepository;

        public HtmlHelper(IUserRepository userRepository, IPlanRepository planRepository, IDictionaryRepository dictionaryRepository)
        {
            _userRepository = userRepository;
            _planRepository = planRepository;
            _dictionaryRepository = dictionaryRepository;
        }

        public IEnumerable<SelectListItem> GetUsersSelectList()
        {
            var selectList = _userRepository.FindByCondition(x => x.IsActive).Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.FirstName} {u.LastName}" });

            return selectList;
        }

        public IEnumerable<SelectListItem> GetUsersSelectListByRole(string role)
        {
            var selectList = _userRepository.FindByCondition(x => x.Role == role && x.IsActive).Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.FirstName} {u.LastName}" });

            return selectList;
        }

        public IEnumerable<SelectListItem> GetUsersSelectListByPlan(int planId)
        {
            var selectList = _planRepository.GetPlanningTeam(planId).Select(u => new SelectListItem { Value = u.UserToPlanId.ToString(), Text = u.FullName });

            return selectList;
        }

        public IEnumerable<SelectListItem> GetPositionsSelectList()
        {
            var selectList = _dictionaryRepository.GetPositions(false).Where(x => x.IsActive).Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title });

            return selectList;
        }

        public IEnumerable<SelectListItem> GetCategoriesSelectList()
        {
            var selectList = _dictionaryRepository.GetStakeholderCategories(false).Where(x => x.IsActive).Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title });

            return selectList;
        }
    }
}
