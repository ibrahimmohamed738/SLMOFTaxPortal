using SLMOFTaxPortal.Models;
using SLMOFTaxPortal.Models.ViewModels;

namespace SLMOFTaxPortal.Services;

public interface IUserManagementService
{
    Task<IEnumerable<UserListViewModel>> GetUsers();
    Task<UserFormViewModel?> GetUserById(int id);
    Task<IEnumerable<RoleViewModel>> GetRoles();

    Task<bool> UsernameExists(string username, int? excludeId = null);
    Task<int> CreateUser(UserFormViewModel model);
    Task<int> UpdateUser(UserFormViewModel model);
    Task<int> ResetPassword(int id, string newPassword);
    Task<int> DeleteUser(int id);
}
