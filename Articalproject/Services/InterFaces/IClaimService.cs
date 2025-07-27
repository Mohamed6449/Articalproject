using Articalproject.Models.Identity;

namespace Articalproject.Services.InterFaces
{
    public interface IClaimService
    {
        public Task<string> AddClaimAsync(Claim claim);
        public Task<string> DeleteClaimAsync(Claim claim);
        public Task<string> UpdateClaimAsync(Claim claim);
		public Task<Claim> GetClaimAsync(int Id);
		public Task<List<Claim>> GetClaimsAsync();






    }
}
