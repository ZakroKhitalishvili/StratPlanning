using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using Core.Entities;
using System.Text;
using Core.Context;
using Application.DTOs;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories
{
    public class FileRepository : RepositoryBase<File>, IFileRepository
    {
        public FileRepository(PlanningDbContext context) : base(context)
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
        /// <summary>
        /// This method deletes also related file answers if exists any
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            var file = Context.Files.Where(x => x.Id == id).Include(x => x.FileAnswers).SingleOrDefault(); ;

            if (file == null)
            {
                return false;
            }
            else
            {
                try
                {
                    Context.FileAnswers.RemoveRange(file.FileAnswers);
                    Delete(file);
                    Context.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        public FileDTO GetFile(int id)
        {
            var file = Get(id);

            return Mapper.Map<FileDTO>(file);
        }
    }
}
