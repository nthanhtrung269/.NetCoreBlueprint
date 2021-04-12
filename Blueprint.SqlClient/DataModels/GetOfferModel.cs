using System;

namespace Blueprint.SqlClient.DataModels
{
    public class GetOfferModel : BaseOfferModel
    {
        public Guid OfferId { get; set; }

        public string CustomerCode { get; set; }

        public string StoreCode { get; set; }
    }
}
