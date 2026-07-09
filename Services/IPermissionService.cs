using SLMOFTaxPortal.Models.ViewModels;

namespace SLMOFTaxPortal.Services;

public interface IPermissionService
{
    Task<IEnumerable<PermissionViewModel>> GetAll();

    Task<PermissionViewModel?> GetById(int id);

    Task<int> Create(PermissionViewModel model);

    Task<int> Update(PermissionViewModel model);

    Task<int> Disable(int id);

    Task<bool> Exists(string permissionCode, int? id = null);
}
