using Blueprint.SqlClient.DataModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace Blueprint.SqlClient.DataServices
{
    public class OfferRepository : IOfferRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OfferRepository> _logger;

        public OfferRepository(IConfiguration configuration,
            ILogger<OfferRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private static class Parameters
        {
            public const string OfferId = "@offerId";
            public const string CustomerCode = "@customerCode";
            public const string StoreCode = "@storeCode";
            public const string ProgramCode = "@programCode";
        }

        private static class StoredProcedures
        {
            public const string GetOnSaleInfoByOfferId = "api_GetOnSaleInfoByOfferId";
            public const string UpdateProgramRunStatus = "api_UpdateProgramRunStatus";
            public const string GetOffers = "api_GetOffers";
        }

        private static class FieldNames
        {
            public const string IsOnSale = "IsOnSale";
            public const string OfferId = "OfferId";
            public const string OfferName = "Name";
            public const string Description = "Description";
            public const string FullDescription = "FullDescription";
            public const string OfferCode = "OfferCode";
            public const string OfferSegment = "OfferSegment";
            public const string OfferSegmentName = "OfferSegmentName";
            public const string AdCodeDesc = "AdCodeDesc";
            public const string ImageUrl = "ImageUrl";
            public const string DiscountPricing = "DiscountPricing";
            public const string Size = "Size";
            public const string Disclaimer = "Disclaimer";
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";
            public const string IsActive = "IsActive";
            public const string MemberAccount = "MemberAccount";
            public const string CustomerCode = "CustomerCode";
            public const string StoreCode = "StoreCode";
        }

        /// <summary>
        /// Gets OnSale info by OfferId.
        /// </summary>
        /// <param name="offerId">The offerId.</param>
        /// <returns>System.Boolean.</returns>
        public bool GetOnSaleInfoByOfferId(Guid offerId)
        {
            bool isOnSale = false;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var command = connection.CreateCommand();
                command.CommandText = StoredProcedures.GetOnSaleInfoByOfferId;
                command.Parameters.AddWithValue(Parameters.OfferId, offerId);

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        isOnSale = DatabaseHelper.GetInteger(reader[FieldNames.IsOnSale]) > 0;
                    }

                    connection.Close();
                }
            }

            return isOnSale;
        }

        /// <summary>
        /// Updates ProgramRun status.
        /// </summary>
        public void UpdateProgramRunStatus()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (var command = new SqlCommand(StoredProcedures.UpdateProgramRunStatus, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets offers.
        /// </summary>
        /// <param name="customerCode">The customerCode.</param>
        /// <param name="storeCode">The storeCode.</param>
        /// <param name="programCode">The programCode.</param>
        /// <returns>List{GetAvailableOfferModel}.</returns>
        public IList<GetOfferModel> GetOffers(string customerCode, string storeCode, string programCode)
        {
            var offers = new List<GetOfferModel>();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = StoredProcedures.GetOffers;
                    command.Parameters.AddWithValue(Parameters.CustomerCode, customerCode);
                    command.Parameters.AddWithValue(Parameters.StoreCode, storeCode);
                    command.Parameters.AddWithValue(Parameters.ProgramCode, programCode);

                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var offer = new GetOfferModel();
                            offer.OfferId = DatabaseHelper.GetGuid(reader[FieldNames.OfferId]);
                            offer.Name = DatabaseHelper.GetString(reader[FieldNames.OfferName]);
                            offer.Description = DatabaseHelper.GetString(reader[FieldNames.Description]);
                            offer.FullDescription = DatabaseHelper.GetString(reader[FieldNames.FullDescription]);
                            offer.OfferCode = DatabaseHelper.GetString(reader[FieldNames.OfferCode]);
                            offer.OfferSegment = DatabaseHelper.GetNullableInteger(reader[FieldNames.OfferSegment]);
                            offer.OfferSegmentName = DatabaseHelper.GetString(reader[FieldNames.OfferSegmentName]);
                            offer.AdCode = DatabaseHelper.GetNullableInteger(reader[FieldNames.FullDescription]);
                            offer.AdCodeDescription = DatabaseHelper.GetString(reader[FieldNames.AdCodeDesc]);
                            offer.ImageUrl = DatabaseHelper.GetString(reader[FieldNames.ImageUrl]);
                            offer.OfferDiscountPricing = DatabaseHelper.GetString(reader[FieldNames.DiscountPricing]);
                            offer.OfferSize = DatabaseHelper.GetString(reader[FieldNames.Size]);
                            offer.OfferDisclaimer = DatabaseHelper.GetString(reader[FieldNames.Disclaimer]);
                            offer.StartDate = DatabaseHelper.GetNullableDateTime(reader[FieldNames.StartDate]);
                            offer.EndDate = DatabaseHelper.GetNullableDateTime(reader[FieldNames.EndDate]);
                            offer.IsActive = DatabaseHelper.GetBoolean(reader[FieldNames.IsActive]);
                            offer.MemberAccount = DatabaseHelper.GetNullableInteger(reader[FieldNames.MemberAccount]);
                            offer.CustomerCode = DatabaseHelper.GetString(reader[FieldNames.CustomerCode]);
                            offer.StoreCode = DatabaseHelper.GetString(reader[FieldNames.StoreCode]);

                            offers.Add(offer);
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Call to {nameof(GetOffers)} failed.");
            }

            return offers;
        }
    }
}
