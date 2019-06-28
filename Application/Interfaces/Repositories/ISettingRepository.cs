using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ISettingRepository : IRepositoryBase<Setting>
    {
        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="index">Setting key</param>
        /// <returns></returns>
        string Get(string index);

        /// <summary>
        /// Sets a new value
        /// </summary>
        /// <param name="index">Setting key</param>
        /// <param name="value">new value</param>
        /// <param name="userId">Invoker user's id</param>
        ///  <returns>Whether accomplished or no</returns>
        bool Set(string index, string value, int userId);

        /// <summary>
        /// Gets a full setting list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SettingDTO> GetSettingList();
    }
}
