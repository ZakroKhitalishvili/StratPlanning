using Application.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using Core.Context;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Application.Repositories
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        public SettingRepository(PlanningDbContext context) : base(context)
        {
        }

        public string Get(string index)
        {
            var setting = Context.Settings.Where(x => x.Index == index).SingleOrDefault();

            if (setting == null || string.IsNullOrEmpty(setting.Value))
            {
                return null;
            }

            return setting.Value;
        }

        public IEnumerable<SettingDTO> GetSettingList()
        {
            return Context.Settings.AsEnumerable().Select(x => Mapper.Map<SettingDTO>(x));
        }

        public bool Set(string index, string value, int userId)
        {
            var setting = Context.Settings.Where(x => x.Index == index).SingleOrDefault();

            if (setting == null)
            {
                return false;
            }

            setting.Value = value;
            setting.UpdatedBy = userId;
            setting.UpdatedAt = DateTime.Now;

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
