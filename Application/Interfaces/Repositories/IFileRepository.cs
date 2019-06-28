using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IFileRepository : IRepositoryBase<File>
    {
        /// <summary>
        /// Creates a new file
        /// </summary>
        /// <param name="name">New file's name</param>
        /// <param name="ext">New file's extension</param>
        /// <param name="path">New file's uploding path</param>
        /// <param name="userId">Invoker user's id</param>
        /// <returns>New file's object</returns>
        FileDTO CreateNewFile(string name, string ext, string path, int userId);

        /// <summary>
        /// Deletea file
        /// </summary>
        /// <param name="id">File's id</param>
        /// <returns>Whether accomplished or no</returns>
        bool Delete(int id);

        /// <summary>
        /// Gets a file
        /// </summary>
        /// <param name="id">File's id</param>
        /// <returns>File's object</returns>
        FileDTO GetFile(int id);
    }
}
