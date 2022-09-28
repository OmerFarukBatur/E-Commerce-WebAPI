using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessageAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
           
            await SendMessageAsync(new[] { to } , subject, body, isBodyHtml);
        }

        public async Task SendMessageAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
             MailMessage mail = new ();
            mail.IsBodyHtml = isBodyHtml;     
            mail.Body = body;
            foreach (var to in tos)
            {
                mail.To.Add(to);
            }
            mail.From = new(_config["Mail:Username"], "Demo E-Ticaret", System.Text.Encoding.UTF8);

            SmtpClient smtp = new();
            smtp.Credentials = new NetworkCredential(_config["Mail:Username"], _config["Mail:Password"]);
            smtp.Port = int.Parse(_config["Mail:Port"]);
            smtp.EnableSsl = true;
            smtp.Host = _config["Mail:Host"];
            await smtp.SendMailAsync(mail);
        }
    }
}
