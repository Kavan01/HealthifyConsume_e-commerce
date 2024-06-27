namespace HealthifyConsume.Areas.Admin.Models
{
    public class ProductModel
    {
        public int? ProductID { get; set; }
        public int? CategoryID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQty { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
    }

}
