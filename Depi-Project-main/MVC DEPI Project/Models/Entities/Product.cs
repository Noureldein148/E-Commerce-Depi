namespace MVC_DEPI_Project.Models.Entities
{
    public class Product
    {
        //[Key ]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; } // FK Category Table (int bardo)
        public Category Category { get; set; }
        // Make EF Understand CategoryID is FK
        public string? ImagePath { get; set; } // 🆕 Add this for product image
        public int Stock { get; set; }
    }
}
