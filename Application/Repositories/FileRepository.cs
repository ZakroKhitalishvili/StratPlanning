using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using Core.Entities;
using System.Text;
using Core.Context;
using Application.DTOs;

namespace Application.Repositories
{
    public class FileRepository: RepositoryBase<File>, IFileRepository
    {
        public FileRepository(PlanningDbContext context):base(context)
        {
        }

        public FileDTO CreateNewFile(string name, string ext, string path, int userId)
        {
            var fileEntity = new File
            {
                Name = name,
                Ext = ext,
                Path = path,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            this.Create(fileEntity);

            this.Context.SaveChanges();

            return new FileDTO
            {
                Id = fileEntity.Id,
                Name = fileEntity.Name,
                Ext = fileEntity.Ext,
                Path = fileEntity.Path
            };
        }
    }
}
