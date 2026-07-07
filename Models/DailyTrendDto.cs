namespace SLMOFTaxPortal.Models;

public class DailyTrendDto
{
    public DateTime TrxDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int TotalTransactions { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public int SuccessfulDeductions { get; set; }
    public int MinistryUpdated { get; set; }

    public string CurrencyName => Currency == "101" ? "USD" : "SLS";
}