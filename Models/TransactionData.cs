namespace SLMOFTaxPortal.Models;

public class TransactionData
{
    public int Id { get; set; }
    public string Tin { get; set; } = string.Empty;
    public string WalletID { get; set; } = string.Empty;
    public string MerchantCode { get; set; } = string.Empty;
    public string TrxID { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime TrxDate { get; set; }
    public int Type { get; set; } = 1;
    public bool Updated { get; set; }
    public string User_id { get; set; } = string.Empty;
    public string DestinationWalletID { get; set; } = string.Empty;
    public decimal TaxAmount { get; set; }
    public DateTime DeductionDate { get; set; }
    public bool UpdatedTax { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string eDahabStatus { get; set; } = string.Empty;
    public string eDahabTransactionCode { get; set; } = string.Empty;
    public string eDahabMessage { get; set; } = string.Empty;
}

