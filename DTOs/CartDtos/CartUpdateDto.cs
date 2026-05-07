namespace RentalService.DTOs.CartDtos
{
    public class CartUpdateDto
    {
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }
        public decimal CartTotal { get; set; }
        public int Count { get; set; }
    }
}
