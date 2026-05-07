namespace RentalService.DTOs.ItemDtos
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; } = "";
        public string? ImagePath { get; set; }
    }
}
