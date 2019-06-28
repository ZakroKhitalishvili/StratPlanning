using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.Constants;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _userName;

        private readonly string _password;

        private readonly string _smtpServer;

        private readonly int _port;

        private readonly string _from;

        public EmailService(ISettingRepository settingRepository)
        {
            _userName = settingRepository.Get(Settings.SmtpUsername);
            _password = settingRepository.Get(Settings.SmtpPassword);
            _smtpServer = settingRepository.Get(Settings.SmtpServer);
            _port = int.Parse(settingRepository.Get(Settings.SmtpPort));
            _from = settingRepository.Get(Settings.SmtpFrom);
        }

        public bool Send(string recipient, string subject, string body)
        {
            SmtpClient client = new SmtpClient();

            client.Host = _smtpServer;
            client.Port = _port;
            client.Credentials = new NetworkCredential(_userName, _password);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(_from);
            message.To.Add(new MailAddress(recipient));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;

            try
            {
                client.Send(message);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                client.Dispose();
            }

            return true;
        }

        public bool SendPasswordToUser(string password, UserDTO user)
        {
            var subject = "Welcome to Strategic Planning";

            var body = $"Hello, {user.FirstName} {user.LastName} <br> Now you can log in our system using Password: <b>{password}</b>";

            return Send(user.Email, subject, body);
        }

        public bool SendPasswordRecoveryInfo(string url, UserDTO user)
        {
            var subject = "Password recovery";

            var body = $"Hello, {user.FirstName} {user.LastName} <br> Here is a link to password recovery page: {url}";

            return Send(user.Email, subject, body);
        }
    }
}
