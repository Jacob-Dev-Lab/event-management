using RentalService.Data;
using RentalService.DTOs.ItemDtos;
using RentalService.Models;
using RentalService.Services.Interfaces;

namespace RentalService.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;

        public ItemService(AppDbContext context)
        {
            _context = context;
        }

        public List<Item> GetAll() => _context.Items.ToList();

        public Item? GetById(int id) => _context.Items.Find(id);

        public async Task<ItemDto?> Create(Item item, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (!ValidateImage(ImageFile)) return null;

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                item.ImagePath = "/images/" + fileName;
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return MapItem(item);
        }

        public async Task<ItemDto?> Update(int id, Item item, IFormFile? imageFile, string existingImage)
        {
            var existing = _context.Items.Find(id);
            if (existing == null) return null;

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!ValidateImage(imageFile)) return null;

                if (!string.IsNullOrEmpty(existing.ImagePath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImagePath.TrimStart('/'));
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                existing.ImagePath = "/images/" + fileName;
            }
            else
            {
                existing.ImagePath = existingImage;
            }

            existing.Name = item.Name;
            existing.Category = item.Category;
            existing.Description = item.Description;
            existing.Price = item.Price;

            await _context.SaveChangesAsync();

            return MapItem(existing);
        }

        public bool Delete(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null) return false;

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

            return true;
        }

        private static bool ValidateImage(IFormFile ImageFile)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(ImageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension)) return false;

            if (ImageFile.Length > 5 * 1024 * 1024) return false;

            return true;
        }

        private ItemDto MapItem(Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                Description = item.Description,
                Price = item.Price,
                FormattedPrice = item.Price.ToString("C"),
                ImagePath = item.ImagePath
            };
        }
    }
}
