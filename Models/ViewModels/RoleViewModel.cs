namespace SLMOFTaxPortal.Models.ViewModels;

public class RoleViewModel
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Active { get; set; }
}
