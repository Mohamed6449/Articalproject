using Articalproject.Models.Identity;

namespace Articalproject.Services.InterFaces
{
    public interface IAccountService
    {
        public (bool canResend, TimeSpan? remainingTime) CanUserResend(string userId, string type);
        public void RecordResend(string userId, string type);

    }
}
