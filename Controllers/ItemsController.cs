using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentalService.Data;
using RentalService.Models;
using RentalService.Services;

[Route("Items")]
public class ItemsController : Controller
{
    private readonly AppDbContext _context;
    private readonly CartService _cartService;

    public ItemsController(AppDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        return View(_context.Items.ToList());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Item item, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetErrors() });

        if (imageFile != null && imageFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, error = "Invalid file type. Only images are allowed." });
            }

            if (imageFile.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, error = "File size exceeds the limit of 5MB." });
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            item.ImagePath = "/images/" + fileName;
        }

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return Json(new { success = true, data = MapItem(item) });
    }

    [HttpGet("Get/{id}")]
    public IActionResult Get(int id)
    {
        var item = _context.Items.Find(id);
        if (item == null)
            return Json(new { success = false });

        return Json(new { success = true, data = MapItem(item) });
    }

    [HttpPost("Update/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, Item item, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetErrors() });

        var existing = _context.Items.Find(id);
        if (existing == null)
            return Json(new { success = false });

        if (imageFile != null && imageFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, error = "Invalid file type. Only images are allowed." });
            }

            if (imageFile.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, error = "File size exceeds the limit of 5MB." });
            }

            if (!string.IsNullOrEmpty(existing.ImagePath))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            existing.ImagePath = "/images/" + fileName;
        }

        existing.Name = item.Name;
        existing.Category = item.Category;
        existing.Description = item.Description;
        existing.Price = item.Price;

        await _context.SaveChangesAsync();

        return Json(new { success = true, data = MapItem(existing) });
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var item = _context.Items.Find(id);
        if (item == null)
            return Json(new { success = false });

        if (!string.IsNullOrEmpty(item.ImagePath))
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        _context.Items.Remove(item);
        _context.SaveChanges();

        return Json(new { success = true });
    }

    [HttpPost("AddToCart")]
    public IActionResult AddToCart(int id, int quantity)
    {
        var item = _context.Items.Find(id);

        if (item == null) return NotFound();

        var cart = _cartService.GetCart();

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ItemId == id);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ItemId = id,
                Quantity = quantity
            });
        }

        _context.SaveChanges();

        return RedirectToAction("Index", "Cart");
    }

    private object ValidateImage(IFormFile imageFile)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(imageFile.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
        {
            return Json (new { success = false, error = "Invalid file type. Only images are allowed." });
        }

        if (imageFile.Length > 5 * 1024 * 1024)
        {
            return Json (new { success = false, error = "File size exceeds the limit of 5MB." });
        }

        return Json (new { success = true });
    }

    private object MapItem(Item item)
    {
        return new
        {
            id = item.Id,
            name = item.Name,
            category = item.Category,
            description = item.Description,
            price = item.Price,
            formattedPrice = item.Price.ToString("C"),
            imagePath = item.ImagePath
        };
    }

    private Dictionary<string, string[]> GetErrors()
    {
        return ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key.Replace("item.", ""),
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );
    }
}