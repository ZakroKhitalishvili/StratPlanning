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
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly IHashService _hashService;

        public UserRepository(PlanningDbContext context, IHashService hashService) : base(context)
        {
            _hashService = hashService;
        }

        public UserDTO AddNew(NewUserDTO newUser, int userId)
        {
            var user = Mapper.Map<User>(newUser);

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.CreatedBy = userId;
            user.UpdatedBy = userId;

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

        public bool Update(UserEditDTO userEdit, int userId)
        {
            var user = Get(userEdit.Id);

            if (user == null)
            {
                return false;
            }

            user.Email = userEdit.Email;
            user.FirstName = userEdit.FirstName;
            user.LastName = userEdit.LastName;
            user.PositionId = userEdit.PositionId;
            user.Role = userEdit.Role;
            user.UpdatedBy = userId;
            user.UpdatedAt = DateTime.Now;

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }


        public IEnumerable<UserDTO> GetUserList(int skipCount, int takeCount, out int totalCount)
        {
            totalCount = Context.Users.Where(x => !x.IsDeleted).Count();

            return Context.Users.Where(x => !x.IsDeleted).Include(x => x.Position).OrderByDescending(x => x.CreatedAt).Skip(skipCount).Take(takeCount).Select(p => Mapper.Map<UserDTO>(p));
        }

        public bool ChangePassword(ChangePasswordDTO changePassword, int userId)
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
            user.UpdatedBy = userId;

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

        public bool ChangePassword(int userEditId, string newPassword, int userId)
        {
            var user = FindByCondition(u => u.Id == userEditId).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            var hashedNewPassword = _hashService.Hash(newPassword);

            user.Password = hashedNewPassword;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = userId;

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
            var user = Context.Users.Where(x => x.Email == email).Include(x => x.Position).SingleOrDefault();

            return (user != null) ? Mapper.Map<UserDTO>(user) : null;
        }

        public UserDTO GetUserById(int id)
        {
            var user = Context.Users.Where(x => x.Id == id).Include(x => x.Position).SingleOrDefault();

            return (user != null) ? Mapper.Map<UserDTO>(user) : null;
        }

        public bool RecoverPassword(RecoverPasswordDTO recoverPassword, int userId)
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
            user.UpdatedBy = userId;

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

            var user = FindByCondition(x => x.Email == email && x.Password == hashedPassword && x.IsActive).FirstOrDefault();

            userDTO = Mapper.Map<UserDTO>(user);

            return user != null;
        }

        public bool UpdateProfile(UserProfileDTO userProfile, int userId)
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
            user.UpdatedBy = userId;

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

        public bool Delete(int id, int userId)
        {
            var user = Get(id);

            if (user != null)
            {
                try
                {
                    user.IsActive = false;
                    user.IsDeleted = true;
                    user.EmailBackUp = user.Email;
                    user.Email = string.Empty;
                    Context.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool Activate(int id, int userId)
        {
            var user = Get(id);

            if (user != null)
            {
                try
                {
                    user.IsActive = true;
                    user.UpdatedAt = DateTime.Now;
                    user.UpdatedBy = userId;
                    Context.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool Disactivate(int id, int userId)
        {
            var user = Get(id);

            if (user != null)
            {
                try
                {
                    user.IsActive = false;
                    user.UpdatedAt = DateTime.Now;
                    user.UpdatedBy = userId;
                    Context.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
