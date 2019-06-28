using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    /// <summary>
    /// Email service interface for sending specific emails to an user
    /// </summary>
    public interface IEmailService
    {
        bool Send(string recipient, string subject, string body);

        bool SendPasswordToUser(string password, UserDTO user);

        bool SendPasswordRecoveryInfo(string url, UserDTO user);
    }
}
