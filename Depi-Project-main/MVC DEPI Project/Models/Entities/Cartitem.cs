namespace MVC_DEPI_Project.Models.Entities
{
    public class Cartitem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }


        public int CartId { get; set; } // FK links cartitem to cart
        public Cart Cart { get; set; }  // not an FK but a way to access the cart from the cartItem

    }
}
