using BulkyBook.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Drawing.Imaging;
using System.Text;

namespace BulkyBook
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSender _emailSender;
        private readonly EmailSettings _emailSettings;
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
                this._emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            try
            {
                var mimeMessage = new MimeMessage();                
                MailboxAddress emailFrom = new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender);
                mimeMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress("", email);
                mimeMessage.To.Add(emailTo);

                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };                

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(_emailSettings.MailServer,_emailSettings.MailPort,MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_emailSettings.Sender, _emailSettings.Password);
                    client.Send(mimeMessage);
                }

            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
