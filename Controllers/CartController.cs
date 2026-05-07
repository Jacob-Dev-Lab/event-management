using Microsoft.AspNetCore.Mvc;
using RentalService.Services.Interfaces;

namespace RentalService.Controllers
{
    [Route("Cart")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var count = _cartService.Add(id, quantity);

            if (count == null) 
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                count
            });
        }

        [HttpPost("UpdateQuantity")]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var data = _cartService.Update(id, quantity);

            if (data == null) return Json(new { success = false } );
            
            return Json(new
            {
                success = true,
                quantity = data.Quantity,
                itemTotal = data.ItemTotal,
                cartTotal = data.CartTotal,
                count = data.Count
            });
        }

        [HttpPost("RemoveFromCart")]
        public IActionResult RemoveFromCart(int id)
        {
            var data = _cartService.Delete(id);

            if (data == null) return Json(new { success = false });

            return Json(new
            {
                success = true,
                count = data.Count,
                total = data.Total
            });
        }

        [HttpGet("GetCount")]
        public IActionResult GetCount()
        {
            var count = _cartService.GetCount();

            if (count == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                count
            });
        }

        [HttpGet("MiniCart")]
        public IActionResult MiniCart()
        {
            var data = _cartService.Mini();

            if (data == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                items = data.Items,
                total = data.Total
            });
        }

    }
}
