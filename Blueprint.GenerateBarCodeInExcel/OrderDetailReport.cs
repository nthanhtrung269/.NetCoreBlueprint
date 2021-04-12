namespace Blueprint.GenerateBarCodeInExcel
{
    public class OrderDetailReport
    {
        public long OrderItemId { get; set; }
        public string ProductUpc { get; set; }
        public string ProductCategory { get; set; }
        public string ProductTitle { get; set; }
        public string OrderNumber { get; set; }
        public string StoreNumber { get; set; }
        public string Dept { get; set; }
        public string Options { get; set; }
        public decimal? Price { get; set; }
    }
}
