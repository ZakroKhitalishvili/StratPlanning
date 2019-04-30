using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
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

        public bool TryAuthentication(string email, string password,out UserDTO userDTO)
        {
            var hashedpassword = _hashService.Hash(password);

            var user = FindByCondition(x => x.Email == email && x.Password == hashedpassword).FirstOrDefault();

            userDTO = Mapper.Map<UserDTO>(user);

            return user != null;
        }

    }
}
