using SLMOFTaxPortal.Models;

namespace SLMOFTaxPortal.Services;

public interface IReportService
{
    Task<DashboardSummary> GetDashboardSummary(ReportFilter filter);
    Task<IEnumerable<DailyTrendDto>> GetDailyTrends(ReportFilter filter);
    Task<IEnumerable<TransactionData>> GetTransactions(ReportFilter filter);
}
