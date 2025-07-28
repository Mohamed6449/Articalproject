using Articalproject.Services.InterFaces;
using System.Net;
using System.Net.Mail;

namespace Articalproject.Services.Implementations
{
    public class EmailSender:IEmailSender
    {
            private readonly IConfiguration _configuration;

            public EmailSender(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task SendEmailAsync(string email, string subject, string htmlMessage)
            {
                var smtpClient = new SmtpClient("smtp.gmail.com") // أو أي SMTP تاني
                {
                    Port = 587,
                    Credentials = new NetworkCredential("mmaraemmm1@gmail.com", "MoM6449666"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("mmaraemmm1@gmail.com"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
        
    }
}
