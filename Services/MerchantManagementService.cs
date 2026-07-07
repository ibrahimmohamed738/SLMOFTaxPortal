namespace SLMOFTaxPortal.Services;

using Dapper;
using Microsoft.Data.SqlClient;
using SLMOFTaxPortal.Models;

public class MerchantManagementService(IConfiguration configuration) : IMerchantManagementService
{
    private readonly string _connectionString =
        configuration.GetConnectionString("RackeDahabDb")!;

    public async Task<IEnumerable<Merchant>> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT TOP (1000)
                UserID,
                MerchantName,
                MerchantNumber,
                MerchantCode,
                City,
                Region,
                Active,
                Tin
            FROM CbsMerchants
            ORDER BY MerchantName;";

        return await connection.QueryAsync<Merchant>(query);
    }

    public async Task<Merchant?> GetByUserId(string userId)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT
                UserID,
                MerchantName,
                MerchantNumber,
                MerchantCode,
                City,
                Region,
                Active,
                Tin
            FROM CbsMerchants
            WHERE UserID = @UserID;";

        return await connection.QueryFirstOrDefaultAsync<Merchant>(
            query,
            new { UserID = userId });
    }

    public async Task<int> Create(Merchant merchant)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            INSERT INTO CbsMerchants
            (
                UserID,
                MerchantName,
                MerchantNumber,
                MerchantCode,
                City,
                Region,
                Active,
                Tin
            )
            VALUES
            (
                @UserID,
                @MerchantName,
                @MerchantNumber,
                @MerchantCode,
                @City,
                @Region,
                @Active,
                @Tin
            );";

        return await connection.ExecuteAsync(query, merchant);
    }

    public async Task<int> Update(Merchant merchant)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            UPDATE CbsMerchants
            SET
                MerchantName = @MerchantName,
                MerchantNumber = @MerchantNumber,
                MerchantCode = @MerchantCode,
                City = @City,
                Region = @Region,
                Active = @Active,
                Tin = @Tin
            WHERE UserID = @UserID;";

        return await connection.ExecuteAsync(query, merchant);
    }

    public async Task<int> Delete(string userId)
    {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            DELETE FROM CbsMerchants
            WHERE UserID = @UserID;";

        return await connection.ExecuteAsync(query, new { UserID = userId });
    }
}