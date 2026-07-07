using SLMOFTaxPortal.Models;

namespace SLMOFTaxPortal.Services;

public interface IMerchantManagementService
{
    Task<IEnumerable<Merchant>> GetAll();
    Task<Merchant?> GetByUserId(string userId);
    Task<int> Create(Merchant merchant);
    Task<int> Update(Merchant merchant);
    Task<int> Delete(string userId);
}