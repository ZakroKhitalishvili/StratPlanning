using Application.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        bool TryAuthentication(string email, string password,out UserDTO user);

        UserDTO AddNew(NewUserDTO user);

        string GeneratePassword();
    }
}
