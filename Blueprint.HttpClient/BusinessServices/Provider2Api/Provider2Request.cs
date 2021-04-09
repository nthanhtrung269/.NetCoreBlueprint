using System.Collections.Generic;

namespace Blueprint.HttpClient1.BusinessServices.Provider2Api
{
    public class Provider2Request
    {
        public int CustomerId { get; set; }

        public int PageNum { get; set; }

        public int PageSize { get; set; }

        public List<string> UpcList { get; set; }

        public List<ProductSortOrderRequestDto> SortOrder { get; set; }

        public List<string> BrandName { get; set; }

        public List<int> DepartmentId { get; set; }

        public List<int> SubDept1Id { get; set; }

        public List<int> SubDept2Id { get; set; }

        public List<int> ItemId { get; set; }

        public List<int> ItemNum { get; set; }

        public string TprType { get; set; }

        public string PromoInd { get; set; }

        public string PrimaryBarcode { get; set; }

        public string Status { get; set; }

        public List<CustomFieldsDto> CustomFields { get; set; }

        public bool DisplayMhSummary { get; set; }

        public bool DisplayAttributeSummary { get; set; }

        public bool DisplayBrandSummary { get; set; }
    }

    public class ProductSortOrderRequestDto
    {
        public int Position { get; set; }

        public string SortColumn { get; set; }

        public string SortDirection { get; set; }
    }

    public class CustomFieldsDto
    {
        public int FieldId { get; set; }

        public string FieldValue { get; set; }
    }
}
