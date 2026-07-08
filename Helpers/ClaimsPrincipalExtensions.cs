using System.Security.Claims;

namespace SLMOFTaxPortal.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static bool HasPermission(this ClaimsPrincipal user, string permission)
    {
        return user.Claims.Any(x =>
            x.Type == "Permission" &&
            x.Value == permission);
    }
}
