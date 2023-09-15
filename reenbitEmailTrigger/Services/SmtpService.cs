using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace reenbitEmailTrigger.Services
{
    internal class SmtpService : ISmtpService
    {
        public void SendEmail(string to, string subject, string body)
        {
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUsername = Environment.GetEnvironmentVariable("SmtpEmail");
            var smtpPassword = Environment.GetEnvironmentVariable("SmtpPassword");
            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var from = new MailAddress(smtpUsername, Environment.GetEnvironmentVariable("SmtpSenderName"));
                var toAddress = new MailAddress(to);
                var message = new MailMessage(from, toAddress)
                {
                    Subject = subject,
                    Body = body,
                };

                client.Send(message);
            }
        }
    }
}
