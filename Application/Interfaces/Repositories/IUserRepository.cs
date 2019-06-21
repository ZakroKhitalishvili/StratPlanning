using Application.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        bool TryAuthentication(string email, string password, out UserDTO user);

        UserDTO AddNew(NewUserDTO user, int userId);

        string GeneratePassword();

        string GetRecoveryToken(string email);

        bool ValidateToken(string token);

        bool RecoverPassword(RecoverPasswordDTO recoverPassword, int userId);

        UserDTO GetUserById(int id);

        UserDTO GetUserByEmail(string email);

        bool UpdateProfile(UserProfileDTO user, int userId);

        bool ChangePassword(ChangePasswordDTO changePassword, int userId);

    }
}
