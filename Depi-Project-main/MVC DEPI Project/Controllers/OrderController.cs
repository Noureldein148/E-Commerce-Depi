using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_DEPI_Project.Data;
using MVC_DEPI_Project.Models;
using MVC_DEPI_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace MVC_DEPI_Project.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Cart GetCart()
        {
            var userId = _userManager.GetUserId(User);
            return _context.Cart
                .Include(c => c.Cartitems)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.UserId == userId);
        }



        public IActionResult Checkout()
        {
            var cart = GetCart();
            return View(cart);
        }
        [HttpPost]
        public IActionResult PlaceOrder()
        {
            var cart = GetCart();
            if (cart == null || !cart.Cartitems.Any())
                return RedirectToAction("Index", "Cart");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        UserId = cart.UserId,
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.Total,
                        OrderItems = new List<OrderItem>()
                    };

                    foreach (var item in cart.Cartitems)
                    {
                        var product = _context.Product.Find(item.ProductId);
                        if (product == null)
                            throw new Exception("Product not found.");

                        if (product.Stock < item.Quantity)
                            throw new Exception($"Not enough stock for {product.Name}.");

                        // Reduce stock
                        product.Stock -= item.Quantity;

                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = product.Price,
                            Total = item.Total
                        });
                    }

                    _context.Order.Add(order);
                    _context.Cartitem.RemoveRange(cart.Cartitems);
                    cart.Subtotal = 0;
                    cart.Total = cart.Shipping;

                    _context.SaveChanges();
                    transaction.Commit();

                    return RedirectToAction("OrderConfirmation");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", ex.Message);
                    return View("Checkout", cart);
                }
            }
        }

    }
}
