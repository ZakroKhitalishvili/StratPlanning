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
            return FindAll().Select(x => x.Title);
        }
    }
}
