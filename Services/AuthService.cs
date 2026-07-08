using Dapper;
using Microsoft.Data.SqlClient;
using SLMOFTaxPortal.Helpers;
using SLMOFTaxPortal.Models;

namespace SLMOFTaxPortal.Services;

public class AuthService(IConfiguration configuration, ILogger<AuthService> logger) : IAuthService
{
    private readonly string _connectionString =
        configuration.GetConnectionString("RackeDahabDb")!;

    public async Task<AppUser?> ValidateUser(string username, string password)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
           SELECT 
    u.Id,
    u.FullName,
    u.Username,
    u.Email,
    u.PasswordHash,
    r.RoleName,
    u.Active
FROM AppUsers u
INNER JOIN AppRoles r ON u.RoleId = r.Id
WHERE u.Username = @Username
  AND u.Active = 1
  AND r.Active = 1;";

        var user = await connection.QueryFirstOrDefaultAsync<AppUser>(
     query,
     new { Username = username });

        if (user == null)
        {
            logger.LogWarning("User {Username} not found.", username);
            return null;
        }

        logger.LogInformation("User found: {Username}", user.Username);
        logger.LogInformation("Stored Hash: {Hash}", user.PasswordHash);

        var valid = PasswordHelper.VerifyPassword(password, user.PasswordHash);

        logger.LogInformation("Password Valid = {Valid}", valid);

        return valid ? user : null;
    }


    public async Task<IEnumerable<string>> GetUserPermissions(int userId)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
        SELECT p.PermissionCode
        FROM AppUsers u
        INNER JOIN AppRoles r ON u.RoleId = r.Id
        INNER JOIN AppRolePermissions rp ON r.Id = rp.RoleId
        INNER JOIN AppPermissions p ON rp.PermissionId = p.Id
        WHERE u.Id = @UserId
          AND u.Active = 1
          AND r.Active = 1
          AND p.Active = 1;";

        return await connection.QueryAsync<string>(
            query,
            new { UserId = userId });
    }
}