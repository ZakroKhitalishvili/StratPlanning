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

        IEnumerable<CategoryDTO> GetCategories();
    }
}
