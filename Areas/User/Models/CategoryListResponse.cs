namespace HealthifyConsume.Areas.User.Models
{
    
    public class Category
    {
        public int categoryID { get; set; }
        public string categoryName { get; set; }
    }

    public class CategoryListResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Category> data { get; set; }
    }
}
