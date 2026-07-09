using Dapper;
using Microsoft.Data.SqlClient;
using SLMOFTaxPortal.Models.ViewModels;

namespace SLMOFTaxPortal.Services;

public class RolePermissionService(IConfiguration configuration) : IRolePermissionService
{
    private readonly string _connectionString =
        configuration.GetConnectionString("RackeDahabDb")!;

    public async Task<IEnumerable<RoleViewModel>> GetRoles()
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT Id, RoleName, Description, Active
            FROM AppRoles
            ORDER BY RoleName;";

        return await connection.QueryAsync<RoleViewModel>(query);
    }

    public async Task<RoleViewModel?> GetRoleById(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT Id, RoleName, Description, Active
            FROM AppRoles
            WHERE Id = @Id;";

        return await connection.QueryFirstOrDefaultAsync<RoleViewModel>(
            query,
            new { Id = id });
    }

    public async Task<int> CreateRole(RoleViewModel model)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            INSERT INTO AppRoles
            (
                RoleName,
                Description,
                Active
            )
            VALUES
            (
                @RoleName,
                @Description,
                @Active
            );";

        return await connection.ExecuteAsync(query, model);
    }

    public async Task<int> UpdateRole(RoleViewModel model)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            UPDATE AppRoles
            SET
                RoleName = @RoleName,
                Description = @Description,
                Active = @Active
            WHERE Id = @Id;";

        return await connection.ExecuteAsync(query, model);
    }

    public async Task<int> DisableRole(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            UPDATE AppRoles
            SET Active = 0
            WHERE Id = @Id;";

        return await connection.ExecuteAsync(query, new { Id = id });
    }

    public async Task<IEnumerable<PermissionViewModel>> GetPermissions()
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT Id, PermissionCode, PermissionName, Description, Active
            FROM AppPermissions
            WHERE Active = 1
            ORDER BY PermissionName;";

        return await connection.QueryAsync<PermissionViewModel>(query);
    }

    public async Task<RolePermissionViewModel?> GetRolePermissions(int roleId)
    {
        using var connection = new SqlConnection(_connectionString);

        const string roleQuery = @"
            SELECT Id, RoleName
            FROM AppRoles
            WHERE Id = @RoleId;";

        var role = await connection.QueryFirstOrDefaultAsync<RoleViewModel>(
            roleQuery,
            new { RoleId = roleId });

        if (role == null)
            return null;

        const string permissionQuery = @"
            SELECT
                p.Id AS PermissionId,
                p.PermissionCode,
                p.PermissionName,
                CASE WHEN rp.Id IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS IsSelected
            FROM AppPermissions p
            LEFT JOIN AppRolePermissions rp
                ON p.Id = rp.PermissionId
               AND rp.RoleId = @RoleId
            WHERE p.Active = 1
            ORDER BY p.PermissionName;";

        var permissions = await connection.QueryAsync<PermissionCheckItem>(
            permissionQuery,
            new { RoleId = roleId });

        return new RolePermissionViewModel
        {
            RoleId = role.Id,
            RoleName = role.RoleName,
            Permissions = permissions.ToList()
        };
    }

    public async Task<int> UpdateRolePermissions(int roleId, List<int> permissionIds)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string deleteQuery = @"
                DELETE FROM AppRolePermissions
                WHERE RoleId = @RoleId;";

            await connection.ExecuteAsync(
                deleteQuery,
                new { RoleId = roleId },
                transaction);

            const string insertQuery = @"
                INSERT INTO AppRolePermissions
                (
                    RoleId,
                    PermissionId
                )
                VALUES
                (
                    @RoleId,
                    @PermissionId
                );";

            foreach (var permissionId in permissionIds.Distinct())
            {
                await connection.ExecuteAsync(
                    insertQuery,
                    new
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    },
                    transaction);
            }

            transaction.Commit();
            return 1;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
