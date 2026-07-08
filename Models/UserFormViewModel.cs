using System.ComponentModel.DataAnnotations;

namespace SLMOFTaxPortal.Models;

public class UserFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    public int RoleId { get; set; }

    public bool Active { get; set; } = true;

    public bool ForceChangePassword { get; set; }

    public string? Password { get; set; }
}