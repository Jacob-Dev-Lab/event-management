using Microsoft.AspNetCore.Mvc;
using RentalService.Data;
using RentalService.Models;

[Route("Items")]
public class ItemsController : Controller
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Items.ToList());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Item item)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetErrors() });

        _context.Items.Add(item);
        _context.SaveChanges();

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
    public IActionResult Update(int id, Item item)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetErrors() });

        var existing = _context.Items.Find(id);
        if (existing == null)
            return Json(new { success = false });

        existing.Name = item.Name;
        existing.Category = item.Category;
        existing.Description = item.Description;
        existing.Price = item.Price;

        _context.SaveChanges();

        return Json(new { success = true, data = MapItem(existing) });
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var item = _context.Items.Find(id);
        if (item == null)
            return Json(new { success = false });

        _context.Items.Remove(item);
        _context.SaveChanges();

        return Json(new { success = true });
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
            formattedPrice = item.Price.ToString("C")
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