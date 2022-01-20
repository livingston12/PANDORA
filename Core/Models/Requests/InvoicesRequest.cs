namespace Pandora.Core.Models.Requests
{
    public class InvoicesRequest
    {
        public int TableId { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public string PaymentMethod { get; set; }
    }
}