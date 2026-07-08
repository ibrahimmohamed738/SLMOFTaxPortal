using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLMOFTaxPortal.Models;
using SLMOFTaxPortal.Services;

namespace SLMOFTaxPortal.Controllers;

[Authorize(Policy = "ViewDashboard")]
public class DashboardController(IReportService reportService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] ReportFilter filter)
    {
        if (filter.FromDate == default)
            filter.FromDate = DateTime.Today.AddDays(-7);

        if (filter.ToDate == default)
            filter.ToDate = DateTime.Today;

        if (string.IsNullOrWhiteSpace(filter.Currency))
            filter.Currency = "101";

        var summary = await reportService.GetDashboardSummary(filter);
        var trends = (await reportService.GetDailyTrends(filter)).ToList();

        ViewBag.Filter = filter;
        ViewBag.Trends = trends;

        return View(summary);
    }
}
