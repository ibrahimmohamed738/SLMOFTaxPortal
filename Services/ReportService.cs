using Dapper;
using Microsoft.Data.SqlClient;
using SLMOFTaxPortal.Models;

namespace SLMOFTaxPortal.Services;

public class ReportService(IConfiguration configuration) : IReportService
{
    private readonly string _connectionString =
        configuration.GetConnectionString("RackeDahabDb")!;

    public async Task<DashboardSummary> GetDashboardSummary(ReportFilter filter)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = $@"
        SELECT
            COUNT(*) AS TotalTransactions,
            ISNULL(SUM(Amount), 0) AS GrossAmount,
            ISNULL(SUM(TaxAmount), 0) AS TaxAmount,
            SUM(CASE WHEN UpdatedTax = 1 THEN 1 ELSE 0 END) AS SuccessfulDeductions,
            SUM(CASE WHEN UpdatedTax = 0 AND eDahabStatus IS NULL THEN 1 ELSE 0 END) AS PendingDeductions,
            SUM(CASE WHEN UpdatedTax = 0 AND eDahabStatus IS NOT NULL THEN 1 ELSE 0 END) AS FailedDeductions,
            SUM(CASE WHEN Updated = 1 THEN 1 ELSE 0 END) AS MinistryUpdated,
            SUM(CASE WHEN Updated = 0 AND UpdatedTax = 1 THEN 1 ELSE 0 END) AS MinistryPending
        FROM cbs_transactions_data
        {BuildWhereClause()};";

        return await connection.QueryFirstAsync<DashboardSummary>(
            query,
            ToParams(filter));
    }

    public async Task<IEnumerable<DailyTrendDto>> GetDailyTrends(ReportFilter filter)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = $@"
        SELECT
            CAST(TrxDate AS DATE) AS TrxDate,
            Currency,
            COUNT(*) AS TotalTransactions,
            ISNULL(SUM(Amount), 0) AS GrossAmount,
            ISNULL(SUM(TaxAmount), 0) AS TaxAmount,
            SUM(CASE WHEN UpdatedTax = 1 THEN 1 ELSE 0 END) AS SuccessfulDeductions,
            SUM(CASE WHEN Updated = 1 THEN 1 ELSE 0 END) AS MinistryUpdated
        FROM cbs_transactions_data
        {BuildWhereClause()}
        GROUP BY CAST(TrxDate AS DATE), Currency
        ORDER BY CAST(TrxDate AS DATE);";

        return await connection.QueryAsync<DailyTrendDto>(
            query,
            ToParams(filter));
    }


    public async Task<IEnumerable<TransactionData>> GetTransactions(ReportFilter filter)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = $@"
        SELECT TOP (500)
            Id,
            Tin,
            WalletID,
            MerchantCode,
            TrxID,
            TrxDate,
            Type,
            Updated,
            User_id,
            Amount,
            DestinationWalletID,
            TaxAmount,
            DeductionDate,
            UpdatedTax,
            eDahabStatus,
            eDahabTransactionCode,
            eDahabMessage,
            Currency
        FROM cbs_transactions_data
        {BuildWhereClause()}
        ORDER BY TrxDate DESC;";

        return await connection.QueryAsync<TransactionData>(
            query,
            ToParams(filter));
    }


    private static string BuildWhereClause()
    {
        return @"
        WHERE TrxDate >= @FromDate
          AND TrxDate < DATEADD(DAY, 1, @ToDate)
          AND Currency = @Currency

          AND (
                @MerchantSearch IS NULL
                OR MerchantCode LIKE '%' + @MerchantSearch + '%'
                OR WalletID LIKE '%' + @MerchantSearch + '%'
              )

          AND (
                @Tin IS NULL
                OR Tin LIKE '%' + @Tin + '%'
              )

          AND (
                @TransactionId IS NULL
                OR TrxID LIKE '%' + @TransactionId + '%'
              )

          AND (
                @DeductionStatus IS NULL
                OR (@DeductionStatus = 'Successful' AND UpdatedTax = 1)
                OR (@DeductionStatus = 'Pending' AND UpdatedTax = 0 AND eDahabStatus IS NULL)
                OR (@DeductionStatus = 'Failed' AND UpdatedTax = 0 AND eDahabStatus IS NOT NULL)
              )

          AND (
                @MinistryStatus IS NULL
                OR (@MinistryStatus = 'Updated' AND Updated = 1)
                OR (@MinistryStatus = 'Pending' AND Updated = 0 AND UpdatedTax = 1)
              )
    ";
    }


    private static object ToParams(ReportFilter filter)
    {
        return new
        {
            FromDate = filter.FromDate.Date,
            ToDate = filter.ToDate.Date,
            Currency = string.IsNullOrWhiteSpace(filter.Currency) ? "101" : filter.Currency,

            MerchantSearch = string.IsNullOrWhiteSpace(filter.MerchantSearch)
                ? null
                : filter.MerchantSearch.Trim(),

            Tin = string.IsNullOrWhiteSpace(filter.Tin)
                ? null
                : filter.Tin.Trim(),

            TransactionId = string.IsNullOrWhiteSpace(filter.TransactionId)
                ? null
                : filter.TransactionId.Trim(),

            DeductionStatus = string.IsNullOrWhiteSpace(filter.DeductionStatus)
                ? null
                : filter.DeductionStatus.Trim(),

            MinistryStatus = string.IsNullOrWhiteSpace(filter.MinistryStatus)
                ? null
                : filter.MinistryStatus.Trim()
        };
    }
}
