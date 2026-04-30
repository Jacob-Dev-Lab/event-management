using Microsoft.EntityFrameworkCore;
using RentalService.Data;
using RentalService.Models;

namespace RentalService.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public Cart GetCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var userId = session.GetString("UserId");

            if (userId == null)
            {
                userId = Guid.NewGuid().ToString();
                session.SetString("UserId", userId);
            }

            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };

                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            return cart;
        }

        public void SaveCart(Cart cart)
        {
            var existingCart = _context.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.Id == cart.Id);

            if (existingCart != null)
            {
                existingCart.Items = cart.Items;
                _context.Carts.Update(existingCart);
            }
            else
            {
                _context.Carts.Add(cart);
            }

            _context.SaveChanges();
        }
    }
}
