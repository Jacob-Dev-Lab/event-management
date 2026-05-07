using RentalService.DTOs.ItemDtos;
using RentalService.Models;

namespace RentalService.Services.Interfaces
{
    public interface IItemService
    {
        List<Item> GetAll();
        Item? GetById(int id);
        Task<ItemDto?> Create(Item item, IFormFile imageFile);
        Task<ItemDto?> Update(int id, Item item, IFormFile? imageFile, string existingImage);
        bool Delete(int id);
    }
}
