namespace SLMOFTaxPortal.Models;

public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool Active { get; set; }
}