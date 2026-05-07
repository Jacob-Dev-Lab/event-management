using Microsoft.AspNetCore.Mvc;
using RentalService.Models;
using RentalService.Services.Interfaces;

[Route("Items")]
public class ItemsController : Controller
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    public IActionResult Index()
    {
        return View(_itemService.GetAll());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Item item, IFormFile ImageFile)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetErrors() });

        var createdItem = await _itemService.Create(item, ImageFile);

        if (createdItem == null)
            return Json(new { success = false });

        return Json(new { success = true, data = createdItem });
    }

    [HttpGet("Get/{id}")]
    public IActionResult Get(int id)
    {
        var item = _itemService.GetById(id);

        if (item == null)
            return Json(new { success = false });

        return Json(new { success = true, data = item });
    }

    [HttpPost("Update/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, Item item, IFormFile? ImageFile, string ExistingImage)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetErrors();
            return Json(new { success = false, errors });
        }

        var updatedItem = await _itemService.Update(id, item, ImageFile, ExistingImage);

        if (updatedItem == null)
            return Json(new { success =  false });

        return Json(new { success = true, data = updatedItem });
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var isDeleted = _itemService.Delete(id);

        if (!isDeleted)
            return Json(new { success = false });

        return Json(new { success = true });
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