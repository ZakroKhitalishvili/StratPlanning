using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IDictionaryRepository
    {
        IEnumerable<DictionaryDTO> GetPositions(bool includeDeleted = true);

        IEnumerable<DictionaryDTO> GetValues(bool includeDeleted = true);

        IEnumerable<DictionaryDTO> GetStakeholderCategories(bool includeDeleted = true);

        IEnumerable<DictionaryDTO> GetStakeholderCriteria(bool includeDeleted = true);

        bool UpdateDictionary(int id, string newTitle, int userId);

        bool Create(DictionaryDTO newDictionary, int userId);

        bool Delete(int id, int userId);

        bool Activate(int id, int userId);

        bool Disactivate(int id, int userId);
    }
}
