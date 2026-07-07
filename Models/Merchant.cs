namespace SLMOFTaxPortal.Models;

public class Merchant
{
    public string UserID { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string MerchantNumber { get; set; } = string.Empty;
    public string MerchantCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string Tin { get; set; } = string.Empty;
}