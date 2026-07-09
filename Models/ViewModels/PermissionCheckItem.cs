namespace SLMOFTaxPortal.Models.ViewModels;

public class PermissionCheckItem
{
    public int PermissionId { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}