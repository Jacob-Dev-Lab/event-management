using RentalService.DTOs;
using RentalService.DTOs.CartDtos;
using RentalService.DTOs.ItemDtos;

namespace RentalService.Services.Interfaces
{
    public interface ICartService
    {
        int? Add(int id, int quantity);
        CartUpdateDto? Update(int id, int quantity);
        RemoveCartItemDto? Delete(int id);
        int? GetCount();
        MiniCartDto? Mini();
    }
}
