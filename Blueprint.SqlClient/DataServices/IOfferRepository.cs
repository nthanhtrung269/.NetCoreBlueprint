using Blueprint.SqlClient.DataModels;
using System;
using System.Collections.Generic;

namespace Blueprint.SqlClient.DataServices
{
    public interface IOfferRepository
    {
        bool GetOnSaleInfoByOfferId(Guid offerId);

        void UpdateProgramRunStatus();

        IList<GetOfferModel> GetOffers(string customerCode, string storeCode, string programCode);
    }
}
