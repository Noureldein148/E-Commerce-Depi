namespace MVC_DEPI_Project.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> ?products { get; set; }
        public string? ImagePath { get; set; }  // <--- Add this for Gategories
    }
}
