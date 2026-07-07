namespace SLMOFTaxPortal.Models;

public class ReportFilter
{
    public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-7);
    public DateTime ToDate { get; set; } = DateTime.Today;

    public string Currency { get; set; } = "101";

    public string? MerchantSearch { get; set; }
    public string? Tin { get; set; }
    public string? DeductionStatus { get; set; }
    public string? MinistryStatus { get; set; }
    public string? TransactionId { get; set; }
}