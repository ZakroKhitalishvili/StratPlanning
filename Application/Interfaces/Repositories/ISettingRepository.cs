using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ISettingRepository : IRepositoryBase<Setting>
    {
        string Get(string index);

        bool Set(string index, string value, int userId);

        IEnumerable<SettingDTO> GetSettingList();
    }
}
