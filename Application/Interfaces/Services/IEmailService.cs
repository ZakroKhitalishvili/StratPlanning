using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        bool SendPasswordToUser(string password,UserDTO user);
    }
}
