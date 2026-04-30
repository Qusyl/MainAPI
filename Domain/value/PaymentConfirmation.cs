
namespace Domain.value
{
    public class PaymentConfirmation
    {
        public Guid PaymentId { get; set; }
        public string ProviderName { get; set; }

        public DateTime ProcessedAt { get; set; }

        public PaymentConfirmation(Guid paymentId, string providerName)
        {
            PaymentId = paymentId;
            ProviderName = providerName;
        }
    }
}
