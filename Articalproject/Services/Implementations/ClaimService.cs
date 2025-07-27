using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;


namespace Articalproject.Services.Implementations
{
	public class ClaimService : IClaimService
	{

		#region Fields
			private readonly IUnitOfWork _unitOfWork;
		#endregion

		#region Constructors
			public ClaimService(IUnitOfWork unitOfWork) { 
				_unitOfWork = unitOfWork;
		
		
		
		}



		#endregion
		#region Handle Functions
		
		public async Task<string> AddClaimAsync(Claim claim)
		{
			try
			{
				await _unitOfWork.Repository<Claim>().AddAsync(claim);
				return "Success";

			}
			catch(Exception ex)
			{

				return ex.Message + "_" + ex.InnerException.Message;

			}




		}

		public async Task<string> DeleteClaimAsync(Claim claim)
		{
			try
			{
				await _unitOfWork.Repository<Claim>().DeleteAsync(claim);
				return "Success";

			}
			catch (Exception ex)
			{

				return ex.Message + "_" + ex.InnerException.Message;

			}

		}

		public async Task<Claim> GetClaimAsync(int Id)
		{
			return await _unitOfWork.Repository<Claim>().GetProductbyIdAsync(Id);
		}

		public async Task<List<Claim>> GetClaimsAsync()
		{

			return await _unitOfWork.Repository<Claim>().GetAsListAsync();


		}

		public async Task<string> UpdateClaimAsync(Claim claim)
		{
			try
			{
				await _unitOfWork.Repository<Claim>().UpdateAsync(claim);
				return "Success";

			}
			catch (Exception ex)
			{

				return ex.Message + "_" + ex.InnerException.Message;

			}


		}
		#endregion
	}
}
