namespace SLMOFTaxPortal.Models;

public class DashboardSummary
{
    public int TotalTransactions { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal TaxAmount { get; set; }

    public int SuccessfulDeductions { get; set; }
    public int PendingDeductions { get; set; }
    public int FailedDeductions { get; set; }

    public int MinistryUpdated { get; set; }
    public int MinistryPending { get; set; }
}