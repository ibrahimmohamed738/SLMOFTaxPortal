namespace SLMOFTaxPortal.Models;

public class UserListViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool ForceChangePassword { get; set; }
}
