using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;


namespace PortalRevendedorProWaiter.Server.Util.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email))
                return Task.CompletedTask;
            return SendEmailsAsync(new string[] { email }, subject, htmlMessage);
        }

        public Task SendEmailsAsync(IEnumerable<string> emails, string subject, string message)
        {
            if (emails.Count() > 0)
            {
                SmtpClient client = new SmtpClient(_emailSettings.PrimaryDomain);
                client.Port = _emailSettings.PrimaryPort;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                client.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_emailSettings.FromEmail);

#if(DEBUG)
                mailMessage.To.Add(_emailSettings.EmailParaDebug);
#else
                foreach (string email in emails)
                    mailMessage.To.Add(email);
#endif

                mailMessage.IsBodyHtml = true;

                mailMessage.Body = message;
                mailMessage.Subject = subject;

                client.Send(mailMessage);
            }
            return Task.CompletedTask;
        }
    }
}