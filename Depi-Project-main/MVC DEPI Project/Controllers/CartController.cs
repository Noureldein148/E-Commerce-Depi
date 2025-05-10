using Microsoft.AspNetCore.Mvc;
using MVC_DEPI_Project.Data;
using MVC_DEPI_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var cart = GetCart();  // Get the current user's cart
        return View(cart);     // Return the view with the cart data
    }

    // Add product to the cart
    public IActionResult AddToCart(int id)
    {
        var cart = GetCart();
        var product = _context.Product.FirstOrDefault(p => p.ID == id);

        if (product != null)
        {
            // Check if the product is already in the cart
            var cartItem = cart.Cartitems.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                // If the product is already in the cart, increase quantity
                cartItem.Quantity++;
                cartItem.Total = cartItem.Quantity * product.Price;
            }
            else
            {
                // Add the new product to the cart
                cart.Cartitems.Add(new Cartitem
                {
                    ProductId = id,
                    Product = product,
                    Quantity = 1,
                    Total = product.Price
                });
            }

            // Update the cart's subtotal and total
            cart.Subtotal = cart.Cartitems.Sum(c => c.Total);
            cart.Total = cart.Subtotal + cart.Shipping;
            _context.SaveChanges();
        }

        // Redirect to the Cart Index page
        return RedirectToAction("Index", "Cart");
    }

    // Get or create the current user's cart
    private Cart GetCart()
    {
        var userId = "some_user_id";  // Example: Use the actual user ID from session or authentication
        var cart = _context.Cart.Include(c => c.Cartitems)
                                .ThenInclude(ci => ci.Product)
                                .FirstOrDefault(c => c.UserId == userId);

        if (cart == null)
        {
            // Create a new cart if none exists
            cart = new Cart
            {
                UserId = userId,
                Subtotal = 0,
                Shipping = 10,  // Example fixed shipping cost
                Total = 0,
                Cartitems = new List<Cartitem>()
            };
            _context.Cart.Add(cart);
            _context.SaveChanges();  // Save the new cart to the database
        }

        return cart;
    }


    // Optionally, implement a RemoveFromCart action
    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart();

        if (cart != null)
        {
            var cartItem = cart.Cartitems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                cart.Cartitems.Remove(cartItem);
                cart.Subtotal = cart.Cartitems.Sum(c => c.Total);
                cart.Total = cart.Subtotal + cart.Shipping;
                _context.SaveChanges();
            }
        }

        return RedirectToAction("Index", "Cart");
    }


}