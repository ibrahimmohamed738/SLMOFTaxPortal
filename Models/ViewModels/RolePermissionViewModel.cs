namespace SLMOFTaxPortal.Models.ViewModels;

public class RolePermissionViewModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<PermissionCheckItem> Permissions { get; set; } = new();
}
