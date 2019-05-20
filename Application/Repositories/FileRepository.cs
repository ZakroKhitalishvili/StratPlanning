using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using Core.Entities;
using System.Text;
using Core.Context;

namespace Application.Repositories
{
    public class FileRepository: RepositoryBase<File>, IFileRepository
    {
        public FileRepository(PlanningDbContext context):base(context)
        {
        }
    }
}
