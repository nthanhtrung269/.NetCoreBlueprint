using System;

namespace Blueprint.AdapterDesignPattern.Models
{
    public class UserPaymentDto
    {
        public Guid UserId { get; set; }

        public int PaymentId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Provider { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CardToken { get; set; }

        public string Expiration { get; set; }

        public string CardNumber { get; set; }

        public string CardType { get; set; }

        public string ExternalId { get; set; }

        public bool IsDefault { get; set; }
    }
}
