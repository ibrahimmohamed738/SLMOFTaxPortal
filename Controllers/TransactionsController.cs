using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLMOFTaxPortal.Models;
using SLMOFTaxPortal.Services;

namespace SLMOFTaxPortal.Controllers;

[Authorize(Policy = "ViewTransactions")]
public class TransactionsController(IReportService reportService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] ReportFilter filter)
    {
        if (filter.FromDate == default)
            filter.FromDate = DateTime.Today.AddDays(-7);

        if (filter.ToDate == default)
            filter.ToDate = DateTime.Today;

        if (string.IsNullOrWhiteSpace(filter.Currency))
            filter.Currency = "101";

        var transactions = (await reportService.GetTransactions(filter)).ToList();

        ViewBag.Filter = filter;

        return View(transactions);
    }
}
