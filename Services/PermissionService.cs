using Dapper;
using Microsoft.Data.SqlClient;
using SLMOFTaxPortal.Models.ViewModels;

namespace SLMOFTaxPortal.Services;

public class PermissionService(
    IConfiguration configuration,
    ILogger<PermissionService> logger)
    : IPermissionService
{
    private readonly string _connectionString =
        configuration.GetConnectionString("RackeDahabDb")!;

    public async Task<IEnumerable<PermissionViewModel>> GetAll()
    {
        const string query = @"
            SELECT
                Id,
                PermissionCode,
                PermissionName,
                Module,
                Description,
                Active
            FROM AppPermissions
            ORDER BY Module,
                     PermissionName;";

        try
        {
            using var connection = new SqlConnection(_connectionString);

            logger.LogInformation("Loading all permissions.");

            return await connection.QueryAsync<PermissionViewModel>(query);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error loading permissions.");

            throw;
        }
    }

    public async Task<PermissionViewModel?> GetById(int id)
    {
        const string query = @"
            SELECT
                Id,
                PermissionCode,
                PermissionName,
                Module,
                Description,
                Active
            FROM AppPermissions
            WHERE Id=@Id;";

        try
        {
            using var connection = new SqlConnection(_connectionString);

            logger.LogInformation(
                "Loading permission {PermissionId}",
                id);

            return await connection.QueryFirstOrDefaultAsync<PermissionViewModel>(
                query,
                new { Id = id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error loading permission {PermissionId}",
                id);

            throw;
        }
    }

    public async Task<bool> Exists(
        string permissionCode,
        int? id = null)
    {
        const string query = @"
            SELECT COUNT(1)
            FROM AppPermissions
            WHERE PermissionCode=@PermissionCode
              AND (@Id IS NULL OR Id<>@Id);";

        using var connection = new SqlConnection(_connectionString);

        var count = await connection.ExecuteScalarAsync<int>(
            query,
            new
            {
                PermissionCode = permissionCode,
                Id = id
            });

        return count > 0;
    }

    public async Task<int> Create(
        PermissionViewModel model)
    {
        const string query = @"
            INSERT INTO AppPermissions
            (
                PermissionCode,
                PermissionName,
                Module,
                Description,
                Active
            )
            VALUES
            (
                @PermissionCode,
                @PermissionName,
                @Module,
                @Description,
                @Active
            );";

        try
        {
            using var connection = new SqlConnection(_connectionString);

            logger.LogInformation(
                "Creating permission {PermissionCode}",
                model.PermissionCode);

            return await connection.ExecuteAsync(
                query,
                model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error creating permission {PermissionCode}",
                model.PermissionCode);

            throw;
        }
    }

    public async Task<int> Update(
        PermissionViewModel model)
    {
        const string query = @"
            UPDATE AppPermissions
            SET
                PermissionCode=@PermissionCode,
                PermissionName=@PermissionName,
                Module=@Module,
                Description=@Description,
                Active=@Active
            WHERE Id=@Id;";

        try
        {
            using var connection = new SqlConnection(_connectionString);

            logger.LogInformation(
                "Updating permission {PermissionId}",
                model.Id);

            return await connection.ExecuteAsync(
                query,
                model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error updating permission {PermissionId}",
                model.Id);

            throw;
        }
    }

    public async Task<int> Disable(int id)
    {
        const string query = @"
            UPDATE AppPermissions
            SET Active=0
            WHERE Id=@Id;";

        try
        {
            using var connection = new SqlConnection(_connectionString);

            logger.LogInformation(
                "Disabling permission {PermissionId}",
                id);

            return await connection.ExecuteAsync(
                query,
                new { Id = id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error disabling permission {PermissionId}",
                id);

            throw;
        }
    }
}
