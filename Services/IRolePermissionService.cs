using SLMOFTaxPortal.Models.ViewModels;

namespace SLMOFTaxPortal.Services;

public interface IRolePermissionService
{
    Task<IEnumerable<RoleViewModel>> GetRoles();
    Task<RoleViewModel?> GetRoleById(int id);
    Task<int> CreateRole(RoleViewModel model);
    Task<int> UpdateRole(RoleViewModel model);
    Task<int> DisableRole(int id);

    Task<IEnumerable<PermissionViewModel>> GetPermissions();
    Task<RolePermissionViewModel?> GetRolePermissions(int roleId);
    Task<int> UpdateRolePermissions(int roleId, List<int> permissionIds);
}