using accounting.src.Core.IService;
using MailKit.Net.Smtp;
using MimeKit;

namespace accounting.src.Service
{
    public class EmailService : IEmailService
    {
        private static MailboxAddress _senderMail = new MailboxAddress("Oksei Accounts","elyi7367@gmail.com");

        public async Task SendEmail(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();
            emailMessage.Subject = subject;
            emailMessage.From.Add(_senderMail);
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Body = new TextPart()
            {
                Text = message
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(_senderMail.Address, "pjmmmmppndqjviot");
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
