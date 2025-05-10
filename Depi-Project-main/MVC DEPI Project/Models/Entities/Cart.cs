namespace MVC_DEPI_Project.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public ICollection<Cartitem> Cartitems { get; set; }
        public decimal Total { get; set; }
        public string UserId { get; internal set; }
    }
}