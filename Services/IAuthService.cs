using SLMOFTaxPortal.Models;

namespace SLMOFTaxPortal.Services;

public interface IAuthService
{
    Task<AppUser?> ValidateUser(string username, string password);
    Task<IEnumerable<string>> GetUserPermissions(int userId);
}
