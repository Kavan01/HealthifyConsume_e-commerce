namespace HealthifyConsume.Areas.User.Models
{
    

    public class Product
    {
        public int productID { get; set; }
        public object categoryID { get; set; }
        public string productName { get; set; }
        public string categoryName { get; set; }
        public Decimal price { get; set; }
        public string description { get; set; }
        public int stockQty { get; set; }
        public int quantity { get; set; }
        public string image { get; set; }
    }

    public class ProductListResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Product> data { get; set; }
    }
}
