using Application.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using Core.Context;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Repositories
{
    public class DictionaryRepository : RepositoryBase<Dictionary>, IDictionaryRepository
    {
        public DictionaryRepository(PlanningDbContext context) : base(context)
        {

        }

        public IEnumerable<DictionaryDTO> GetPositions(bool includeDeleted = true)
        {
            return FindByCondition(x => x.HasPosition && (x.IsDeleted == includeDeleted || !x.IsDeleted)).AsEnumerable().Select(p => Mapper.Map<DictionaryDTO>(p)).ToList();
        }

        public IEnumerable<DictionaryDTO> GetValues(bool includeDeleted = true)
        {
            return FindByCondition(x => x.HasValue && (x.IsDeleted == includeDeleted || !x.IsDeleted)).AsEnumerable().Select(x => Mapper.Map<DictionaryDTO>(x));
        }

        public IEnumerable<DictionaryDTO> GetStakeholderCriteria(bool includeDeleted = true)
        {
            return FindByCondition(x => x.HasStakeholderCriteria && ( x.IsDeleted == includeDeleted || !x.IsDeleted)).AsEnumerable().Select(p => Mapper.Map<DictionaryDTO>(p)).ToList();
        }

        public IEnumerable<DictionaryDTO> GetStakeholderCategories(bool includeDeleted = true)
        {
            return FindByCondition(x => x.HasStakeholderCategory && (x.IsDeleted == includeDeleted || !x.IsDeleted)).AsEnumerable().Select(p => Mapper.Map<DictionaryDTO>(p)).ToList();
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

        public bool Delete(int id, int userId)
        {
            var dictionary = Get(id);

            if (dictionary != null)
            {
                try
                {
                    dictionary.IsActive = false;
                    dictionary.IsDeleted = true;
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

        public bool Activate(int id,int userId)
        {
            var dictionary = Get(id);

            if (dictionary != null)
            {
                try
                {
                    dictionary.IsActive = true;
                    dictionary.UpdatedAt = DateTime.Now;
                    dictionary.UpdatedBy = userId;
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

        public bool Disactivate(int id, int userId)
        {
            var dictionary = Get(id);

            if (dictionary != null)
            {
                try
                {
                    dictionary.IsActive = false;
                    dictionary.UpdatedAt = DateTime.Now;
                    dictionary.UpdatedBy = userId;
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
