using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.Context;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly IHashService _hashService;

        public UserRepository(PlanningDbContext context, IHashService hashService) : base(context)
        {
            _hashService = hashService;
        }

        public bool AuthenticateUser(string email, string password)
        {
            var hashedpassword = _hashService.Hash(password);

            var user = FindByCondition(x => x.Email == email && x.Password == hashedpassword).FirstOrDefault();

            return user != null;
        }

    }
}
