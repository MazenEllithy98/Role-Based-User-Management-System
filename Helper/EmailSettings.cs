using Demo.DAL.Models;
using System.Net;
using System;
using MailKit;
using Demo.PL.Settings;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace Demo.PL.Helper
{
    public class EmailSettings : IEmailSettings
    {
        private readonly MailSettings _options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            _options = options.Value;

        }

        public void SendEmail(Email email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = email.Subject,

            };
            mail.To.Add(MailboxAddress.Parse(email.To));
            var builder = new BodyBuilder();
            builder.TextBody = email.Body;
            mail.Body= builder.ToMessageBody();
            builder.HtmlBody = email.Body;
            mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));
            using var smtp = new SmtpClient();
            smtp.Connect(_options.Host, _options.Port , SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Email , _options.Password);
            smtp.Send(mail);
            smtp.Disconnect(true);
        }

    }
}
