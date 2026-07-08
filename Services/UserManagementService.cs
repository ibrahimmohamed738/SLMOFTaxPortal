using Dapper;
using Microsoft.Data.SqlClient;
using SLMOFTaxPortal.Helpers;
using SLMOFTaxPortal.Models;

namespace SLMOFTaxPortal.Services;

public class UserManagementService(IConfiguration configuration) : IUserManagementService
{
    private readonly string _connectionString =
        configuration.GetConnectionString("RackeDahabDb")!;

    public async Task<IEnumerable<UserListViewModel>> GetUsers()
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT
                u.Id,
                u.FullName,
                u.Username,
                u.Email,
                r.RoleName,
                u.Active,
                u.FailedLoginCount,
                u.LockoutEnd,
                u.LastLoginAt,
                u.ForceChangePassword
            FROM AppUsers u
            INNER JOIN AppRoles r ON u.RoleId = r.Id
            ORDER BY u.FullName;";

        return await connection.QueryAsync<UserListViewModel>(query);
    }

    public async Task<UserFormViewModel?> GetUserById(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT
                Id,
                FullName,
                Username,
                Email,
                RoleId,
                Active,
                ForceChangePassword
            FROM AppUsers
            WHERE Id = @Id;";

        return await connection.QueryFirstOrDefaultAsync<UserFormViewModel>(
            query,
            new { Id = id });
    }

    public async Task<IEnumerable<RoleViewModel>> GetRoles()
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT Id, RoleName
            FROM AppRoles
            WHERE Active = 1
            ORDER BY RoleName;";

        return await connection.QueryAsync<RoleViewModel>(query);
    }

    public async Task<bool> UsernameExists(string username, int? excludeId = null)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT COUNT(1)
            FROM AppUsers
            WHERE Username = @Username
              AND (@ExcludeId IS NULL OR Id <> @ExcludeId);";

        var count = await connection.ExecuteScalarAsync<int>(
            query,
            new { Username = username, ExcludeId = excludeId });

        return count > 0;
    }

    public async Task<int> CreateUser(UserFormViewModel model)
    {
        using var connection = new SqlConnection(_connectionString);

        var passwordHash = PasswordHelper.HashPassword(model.Password!);

        const string query = @"
            INSERT INTO AppUsers
            (
                FullName,
                Username,
                Email,
                PasswordHash,
                RoleId,
                Active,
                CreatedAt,
                FailedLoginCount,
                ForceChangePassword,
                PasswordChangedAt
            )
            VALUES
            (
                @FullName,
                @Username,
                @Email,
                @PasswordHash,
                @RoleId,
                @Active,
                GETDATE(),
                0,
                @ForceChangePassword,
                GETDATE()
            );";

        return await connection.ExecuteAsync(query, new
        {
            model.FullName,
            model.Username,
            model.Email,
            PasswordHash = passwordHash,
            model.RoleId,
            model.Active,
            model.ForceChangePassword
        });
    }

    public async Task<int> UpdateUser(UserFormViewModel model)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            UPDATE AppUsers
            SET
                FullName = @FullName,
                Username = @Username,
                Email = @Email,
                RoleId = @RoleId,
                Active = @Active,
                ForceChangePassword = @ForceChangePassword
            WHERE Id = @Id;";

        return await connection.ExecuteAsync(query, model);
    }

    public async Task<int> ResetPassword(int id, string newPassword)
    {
        using var connection = new SqlConnection(_connectionString);

        var passwordHash = PasswordHelper.HashPassword(newPassword);

        const string query = @"
            UPDATE AppUsers
            SET
                PasswordHash = @PasswordHash,
                FailedLoginCount = 0,
                LockoutEnd = NULL,
                PasswordChangedAt = GETDATE(),
                ForceChangePassword = 1
            WHERE Id = @Id;";

        return await connection.ExecuteAsync(query, new
        {
            Id = id,
            PasswordHash = passwordHash
        });
    }

    public async Task<int> DeleteUser(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            UPDATE AppUsers
            SET Active = 0
            WHERE Id = @Id;";

        return await connection.ExecuteAsync(query, new { Id = id });
    }
}