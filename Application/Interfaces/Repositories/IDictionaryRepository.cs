using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IDictionaryRepository
    {
        /// <summary>
        /// Get dictionaries of position's type
        /// </summary>
        /// <param name="includeDeleted">Determines whether to include deleted dictioanries</param>
        /// <returns></returns>
        IEnumerable<DictionaryDTO> GetPositions(bool includeDeleted = true);

        /// <summary>
        /// Get dictionaries of values's type
        /// </summary>
        /// <param name="includeDeleted">Determines whether to include deleted dictioanries</param>
        /// <returns></returns>
        IEnumerable<DictionaryDTO> GetValues(bool includeDeleted = true);

        /// <summary>
        /// Get dictionaries of stakeholder category's type
        /// </summary>
        /// <param name="includeDeleted">Determines whether to include deleted dictioanries</param>
        /// <returns></returns>
        IEnumerable<DictionaryDTO> GetStakeholderCategories(bool includeDeleted = true);

        /// <summary>
        /// Get dictionaries of stakehodler criterion's type
        /// </summary>
        /// <param name="includeDeleted">Determines whether to include deleted dictioanries</param>
        /// <returns></returns>
        IEnumerable<DictionaryDTO> GetStakeholderCriteria(bool includeDeleted = true);

        /// <summary>
        /// Updates a dictionary
        /// </summary>
        /// <param name="id">Dictionary's id</param>
        /// <param name="newTitle">Updated value</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool UpdateDictionary(int id, string newTitle, int userId);

        /// <summary>
        /// Creates a new dictionary
        /// </summary>
        /// <param name="newDictionary">New dictioanry data</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Create(DictionaryDTO newDictionary, int userId);

        /// <summary>
        /// Deletes a dictionary
        /// </summary>
        /// <param name="id">Dictioanry's id</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Delete(int id, int userId);

        /// <summary>
        /// Activates a dictionary
        /// </summary>
        /// <param name="id">Dictionary's id</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Activate(int id, int userId);

        /// <summary>
        /// Deactivates a dictionary
        /// </summary>
        /// <param name="id">Dictionary's id</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Disactivate(int id, int userId);
    }
}
