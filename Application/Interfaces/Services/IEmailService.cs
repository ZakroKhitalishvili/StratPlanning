using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    /// <summary>
    /// Email interface for sending specific emails to users
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// General purpose method for sending emails
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        bool Send(string recipient, string subject, string body);

        /// <summary>
        /// Sends a plain password to an user
        /// </summary>
        /// <param name="password"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        bool SendPasswordToUser(string password, UserDTO user);

        /// <summary>
        /// Sends a password recovery link to an user
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        bool SendPasswordRecoveryInfo(string url, UserDTO user);
    }
}
