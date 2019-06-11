using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IDictionaryRepository
    {
        IEnumerable<PositionDTO> GetPositions();

        IEnumerable<string> GetValues();

        IEnumerable<CategoryDTO> GetStakeholderCategories();

        IEnumerable<CriterionDTO> GetCriterions();

        bool UpdateDictionary(int id, string newTitle, int userId);

        bool Create(DictionaryDTO newDictionary, int userId);

        bool Delete(int id);
    }
}
