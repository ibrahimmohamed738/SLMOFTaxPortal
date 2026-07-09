namespace SLMOFTaxPortal.Models.ViewModels;

public class PermissionViewModel
{
    public int Id { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Active { get; set; }
}
