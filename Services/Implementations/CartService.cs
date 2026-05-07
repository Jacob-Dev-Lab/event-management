using Microsoft.EntityFrameworkCore;
using RentalService.Data;
using RentalService.DTOs.CartDtos;
using RentalService.DTOs.ItemDtos;
using RentalService.Models;
using RentalService.Services.Interfaces;

namespace RentalService.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public Cart? GetCart()
        {
            var session = (_httpContextAccessor.HttpContext?.Session);

            if (session == null) return null;

            var userId = session.GetString("UserId");

            if (userId == null)
            {
                userId = Guid.NewGuid().ToString();
                session.SetString("UserId", userId);
            }

            var cart = _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Item)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = []
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

        public int? Add(int id, int quantity)
        {
            var item = _context.Items.Find(id);

            if (item == null) return null;

            var cart = GetCart();
            if (cart == null || quantity == 0) return null;

            var existingItem = cart.Items.FirstOrDefault(i => i.ItemId == id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ItemId = id,
                    Item = item,
                    Quantity = quantity
                });
            }

            SaveCart(cart);

            var totalCount = cart.Items.Sum(i => i.Quantity);

            return totalCount;
        }

        public CartUpdateDto? Update(int id, int quantity)
        {
            var cart = GetCart();
            if (cart == null) return null;

            var item = cart.Items.FirstOrDefault(i => i.ItemId == id);
            if (item == null) return null;

            if (quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = quantity;

            var itemTotal = item.Item.Price * quantity;
            var cartTotal = cart.Items.Sum(ci => ci.Quantity * ci.Item.Price);
            var count = cart.Items.Sum(ci => ci.Quantity);

            SaveCart(cart);

            return new CartUpdateDto
            {
                Quantity = quantity,
                ItemTotal = itemTotal,
                CartTotal = cartTotal,
                Count = count
            };
        }

        public RemoveCartItemDto? Delete(int id)
        {
            var cart = GetCart();
            if (cart == null) return null;

            var item = cart.Items.FirstOrDefault(i => i.ItemId == id);
            if (item == null) return null;

            cart.Items.Remove(item);

            SaveCart(cart);

            var cartTotal = cart.Items.Sum(ci => ci.Quantity * ci.Item.Price);
            var count = cart.Items.Sum(ci => ci.Quantity);

            return new RemoveCartItemDto
            {
                Count = count,
                Total = cartTotal.ToString("C")
            };
        }

        public int? GetCount()
        {
            var cart = GetCart();
            if (cart == null) return null;

            var totalCount = cart.Items.Sum(i => i.Quantity);

            return totalCount;
        }

        public MiniCartDto? Mini()
        {
            var cart = GetCart();
            if (cart == null) return null;

            var items = cart.Items.Select(ci => new CartItemDto
            {
                Id = ci.Item.Id,
                Name = ci.Item.Name,
                Price = ci.Item.Price,
                Quantity = ci.Quantity,
                Image = ci.Item.ImagePath,
            }).ToList();

            var total = cart.Items.Sum(i => i.Item.Price * i.Quantity);

            return new MiniCartDto
            {
                Items = items,
                Total = total.ToString("C")
            };
        }
    }
}
