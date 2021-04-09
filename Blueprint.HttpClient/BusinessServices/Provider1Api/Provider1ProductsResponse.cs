using System.Collections.Generic;

namespace Blueprint.HttpClient1.BusinessServices.Provider1Api
{
    public class Provider1ProductsResponse
    {
        public List<Provider1Product> Products { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public string TotalPrice { get; set; }

        public int TotalQuantity { get; set; }

        public int ItemCount { get; set; }
    }

    public class Provider1Product
    {
        public int Id { get; set; }

        public string Sku { get; set; }

        public string ItemType { get; set; }

        public string ItemKey { get; set; }

        public string Brand { get; set; }

        public int BrandId { get; set; }

        public string Name { get; set; }

        public string Size { get; set; }

        public Provider1Category SpecificCategory { get; set; }

        public Provider1Category DisplayCategory { get; set; }

        public string Category { get; set; }

        public int CategoryId { get; set; }

        public int UniversalCategoryId { get; set; }

        public string UniversalCategoryName { get; set; }

        public bool IsSubStoreCategory { get; set; }

        public string CurrentPrice { get; set; }

        public string RegularPrice { get; set; }

        public string CurrentUnitPrice { get; set; }

        public string Description { get; set; }

        public string Aisle { get; set; }

        public bool InStock { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsAvailableInStore { get; set; }

        public Provider1ManufacturerInformation ManufacturerInformation { get; set; }

        public List<Provider1Label> Labels { get; set; }

        public bool HasLabels { get; set; }

        public List<Provider1Link> ImageLinks { get; set; }

        public List<Provider1Link> Links { get; set; }

        public List<Provider1VariationLink> VariationLinks { get; set; }

        public Provider1PointRedemptionInfo PointRedemptionInfo { get; set; }
    }

    public class Provider1Category
    {
        public string CategoryName { get; set; }

        public int CategoryId { get; set; }

        public string UniversalCategoryName { get; set; }

        public int UniversalCategoryId { get; set; }

        public bool IsSubStoreCategory { get; set; }
    }

    public class Provider1ManufacturerInformation
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
    }

    public class Provider1Label
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }

    public class Provider1Link
    {
        public string Rel { get; set; }

        public List<string> Placeholders { get; set; }

        public string Uri { get; set; }
    }

    public class Provider1VariationLink
    {
        public string Name { get; set; }

        public List<Provider1Link> Links { get; set; }
    }

    public class Provider1PointRedemptionInfo
    {
        public int NumberOfPoints { get; set; }

        public string ReducedPrice { get; set; }

        public int MaxRedeemableQuantity { get; set; }

        public string FrequentShopperProgramLabel { get; set; }

        public string DateText { get; set; }

        public string PriceText { get; set; }

        public string MaxRedeemableQuantityText { get; set; }
    }
}
