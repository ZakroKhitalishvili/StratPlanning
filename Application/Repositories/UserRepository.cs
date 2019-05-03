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

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

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

        public bool ChangePassword(ChangePasswordDTO changePassword)
        {
            if (changePassword.NewPassword != changePassword.ConfirmNewPassword)
            {
                return false;
            }
            var hashedPassword = _hashService.Hash(changePassword.Password);

            var user = FindByCondition(u => u.Id == changePassword.Id && u.Password == hashedPassword).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            var hashedNewPassword = _hashService.Hash(changePassword.NewPassword);

            user.Password = hashedNewPassword;
            user.UpdatedAt = DateTime.Now;

            try
            {
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public string GeneratePassword()
        {
            var random = new Random();

            var length = 6;

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var sym = ((char)random.Next('a', 'z' + 1)).ToString();

                if (random.Next(0, 2) == 0)
                {
                    sym = sym.ToUpper();
                }

                stringBuilder.Append(sym);
            }

            return stringBuilder.ToString();

        }

        public string GetRecoveryToken(string email)
        {
            var user = FindByCondition(x => x.Email == email).FirstOrDefault();

            if (user == null)
            {
                return string.Empty;
            }
            else
            {
                var randomString = GeneratePassword();
                var token = _hashService.Hash(randomString);
                user.Token = token;
                user.TokenExpiration = DateTime.Now.AddDays(1);

                try
                {
                    Save();
                }
                catch (Exception)
                {
                    return string.Empty;
                }

                return token;
            }
        }

        public UserDTO GetUserByEmail(string email)
        {
            var user = FindByCondition(x => x.Email == email).FirstOrDefault();

            return (user != null) ? Mapper.Map<UserDTO>(user) : null;
        }

        public UserDTO GetUserById(int id)
        {
            var user = Get(id);

            return (user != null) ? Mapper.Map<UserDTO>(user) : null;
        }

        public bool RecoverPassword(RecoverPasswordDTO recoverPassword)
        {
            if (!(recoverPassword.NewPassword == recoverPassword.ConfirmNewPassword))
            {
                return false;
            }

            if (!ValidateToken(recoverPassword.Token))
            {
                return false;
            }

            var user = FindByCondition(x => x.Token == recoverPassword.Token).First();

            user.Password = _hashService.Hash(recoverPassword.NewPassword);

            user.Token = null;

            user.TokenExpiration = null;

            user.UpdatedAt = DateTime.Now;

            try
            {
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool TryAuthentication(string email, string password, out UserDTO userDTO)
        {
            var hashedPassword = _hashService.Hash(password);

            var user = FindByCondition(x => x.Email == email && x.Password == hashedPassword).FirstOrDefault();

            userDTO = Mapper.Map<UserDTO>(user);

            return user != null;
        }

        public bool UpdateProfile(UserProfileDTO userProfile)
        {
            if (FindByCondition(u => u.Email == userProfile.Email && u.Id != userProfile.Id).Any())
            {
                return false;
            }

            var user = Get(userProfile.Id);

            user.FirstName = userProfile.FirstName;
            user.LastName = userProfile.LastName;
            user.Email = userProfile.Email;
            user.UpdatedAt = DateTime.Now;

            try
            {
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            if (FindByCondition(x => x.Token == token && x.TokenExpiration > DateTime.Now).Any())
            {
                return true;
            }

            return false;
        }
    }
}
