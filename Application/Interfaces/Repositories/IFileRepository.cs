using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IFileRepository : IRepositoryBase<File>
    {
        FileDTO CreateNewFile(string name, string ext, string path, int userId);
    }
}
