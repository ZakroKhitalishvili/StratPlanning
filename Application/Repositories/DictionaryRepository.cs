using Application.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using Core.Context;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Repositories
{
    public class DictionaryRepository : RepositoryBase<Dictionary>, IDictionaryRepository
    {
        public DictionaryRepository(PlanningDbContext context) : base(context)
        {

        }

        public IEnumerable<PositionDTO> GetPositions()
        {
            return FindByCondition(x => x.HasPosition).AsEnumerable().Select(p => Mapper.Map<PositionDTO>(p)).ToList();
        }

        public IEnumerable<string> GetValues()
        {
            return FindByCondition(x => x.HasValue).AsEnumerable().Select(x => x.Title);
        }

        public IEnumerable<CriterionDTO> GetCriterions()
        {
            return FindByCondition(x => x.HasStakeholderCriteria).AsEnumerable().Select(p => Mapper.Map<CriterionDTO>(p)).ToList();
        }

        public IEnumerable<CategoryDTO> GetStakeholderCategories()
        {
            return FindByCondition(x => x.HasStakeholderCategory).AsEnumerable().Select(p => Mapper.Map<CategoryDTO>(p)).ToList();
        }

        public bool UpdateDictionary(int id, string newTitle, int userId)
        {
            var dictionary = Get(id);

            if (dictionary != null)
            {
                dictionary.Title = newTitle;
                dictionary.UpdatedAt = DateTime.Now;
                dictionary.UpdatedBy = userId;
                try
                {

                    Context.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public bool Create(DictionaryDTO newDictionary, int userId)
        {
            var dictionary = Mapper.Map<Dictionary>(newDictionary);

            dictionary.CreatedAt = DateTime.Now;
            dictionary.UpdatedAt = DateTime.Now;
            dictionary.CreatedBy = userId;
            dictionary.UpdatedBy = userId;

            try
            {
                Create(dictionary);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool Delete(int id)
        {
            var dictionary = Get(id);

            if (dictionary != null)
            {
                try
                {
                    Delete(dictionary);
                    Context.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
