using System;
using System.Collections.Generic;

namespace Blueprint.HttpClient1.BusinessServices.Provider2Api
{
    public class Provider2Response
    {
        public Provider2Page Page { get; set; }

        public List<Provider2MhSummary> MhSummary { get; set; }

        public List<string> BrandSummary { get; set; }
    }

    public class Provider2Page
    {
        public List<Provider2Content> Content { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalElements { get; set; }

        public int TotalPages { get; set; }

        public bool FirstPage { get; set; }

        public bool LastPage { get; set; }
    }

    public class Provider2Content
    {
        public string KeyId { get; set; }

        public int ItemId { get; set; }

        public string ItemNum { get; set; }

        public string NameForSales { get; set; }

        public string NameForPurchases { get; set; }

        public string GfItemDescription { get; set; }

        public string BrandName { get; set; }

        public string BarcodeNum { get; set; }

        public int BarcodeTypeId { get; set; }

        public string Upc { get; set; }

        public Provider2SalesUom SalesUom { get; set; }

        public string Description { get; set; }

        public int ClassId { get; set; }

        public int ClassXref { get; set; }

        public int DepartmentId { get; set; }

        public string Department { get; set; }

        public int SubDept1Id { get; set; }

        public int SubDept1Parent { get; set; }

        public string SubDepartment1 { get; set; }

        public int SubDept2Id { get; set; }

        public int SubDept2Parent { get; set; }

        public string SubDepartment2 { get; set; }

        public Provider2StorageArea StorageArea { get; set; }

        public string Status { get; set; }

        public Provider2IssueUom IssueUom { get; set; }

        public Provider2ItemClass ItemClass { get; set; }

        public string AllowRetail { get; set; }

        public string PrimaryBarcode { get; set; }

        public int CustomerId { get; set; }

        public string RegType { get; set; }

        public string RegDesc { get; set; }

        public string RegPriceMulti { get; set; }

        public decimal RegPrice { get; set; }

        public decimal PromoInd { get; set; }

        public DateTime PriceDate { get; set; }

        public DateTime LoadDate { get; set; }

        public DateTime RegStart { get; set; }

        public DateTime RegEnd { get; set; }

        public int Movement { get; set; }

        public string ImageUrl { get; set; }

        public string ImageUrlSource { get; set; }

        public string MwgProductGuid { get; set; }

        public DateTime LastUpdate { get; set; }

        public string UnitSize { get; set; }

        public List<Provider2ItemAttribute> ItemAttributes { get; set; }
    }

    public class Provider2SalesUom
    {
        public int QtyUomId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }

    public class Provider2StorageArea
    {
        public int StorageAreaId { get; set; }

        public string StorageAreaNum { get; set; }

        public string StorageAreaName { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }

    public class Provider2IssueUom
    {
        public int QtyUomId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }

    public class Provider2ItemClass
    {
        public int ItemClassId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }

    public class Provider2ItemAttribute
    {
        public Provider2ItemAttributeId Id { get; set; }

        public string FieldValue { get; set; }
    }

    public class Provider2ItemAttributeId
    {
        public int ItemId { get; set; }
    }

    public class Provider2MhSummary
    {
        public int DepartmentId { get; set; }

        public string Department { get; set; }

        public List<Provider2SubDept1> SubDept1s { get; set; }
    }

    public class Provider2SubDept1
    {
        public int SubDept1Id { get; set; }

        public int SubDept1Parent { get; set; }

        public string SubDepartment1 { get; set; }

        public List<Provider2SubDept2> SubDept2s { get; set; }
    }

    public class Provider2SubDept2
    {
        public int SubDept2Id { get; set; }

        public int SubDept2Parent { get; set; }

        public string SubDepartment2 { get; set; }
    }
}
