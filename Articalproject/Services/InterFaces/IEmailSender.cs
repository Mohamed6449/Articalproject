namespace Articalproject.Services.InterFaces
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage);

    }
}
