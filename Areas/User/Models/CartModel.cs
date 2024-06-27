namespace HealthifyConsume.Areas.User.Models
{
    public class CartModel
    {
        public int CartID { get; set; }

        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        /*public string? ProductImage { get; set; }*/
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public int UserID { get; set; }
        public decimal? TotalAmount { get; set;}
    }

    
}
