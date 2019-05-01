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

        public UserDTO AddNew(NewUserDTO newUser)
        {
            var user = Mapper.Map<User>(newUser);

            user.Password = _hashService.Hash(user.Password);

            if (FindByCondition(u => u.Email == user.Email).Any())
            {
                return null;
            }

            try
            {
                Create(user);
                Save();
            }
            catch (Exception)
            {
                return null;
            }

            return Mapper.Map<UserDTO>(user);

        }

        public string GeneratePassword()
        {
            var random = new Random();

            var length = 40;

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var sym=((char)random.Next('a', 'z' + 1)).ToString();
                
                if(random.Next(0,2)==0)
                {
                    sym = sym.ToUpper();
                }

                stringBuilder.Append(sym);
            }

            return stringBuilder.ToString();

        }

        public bool TryAuthentication(string email, string password, out UserDTO userDTO)
        {
            var hashedPassword = _hashService.Hash(password);

            var user = FindByCondition(x => x.Email == email && x.Password == hashedPassword).FirstOrDefault();

            userDTO = Mapper.Map<UserDTO>(user);

            return user != null;
        }



    }
}
