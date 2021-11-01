using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using DocumentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Mail;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            this.mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            //email.Sender = MailboxAddress.Parse(mailSettings.Mail); //send without sender name
            email.Sender = (new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            //message.From.Add (new MailboxAddress ("Joey Tribbiani", "joey@friends.com"));
            //message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", "chandler@friends.com"));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            //add mail attachement
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            //email.Body = new TextPart(TextFormat.Html) { Text = "<h1>Example HTML Message Body</h1>" };
            //send mail
            using var smtp = new SmtpClient();
            smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
