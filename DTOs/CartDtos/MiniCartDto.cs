namespace RentalService.DTOs.CartDtos
{
    public class MiniCartDto
    {
        public List<CartItemDto> Items { get; set; } = [];
        public string Total { get; set; } = string.Empty;
    }
}
