namespace RentalService.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } = new Cart();
        public int ItemId { get; set; }
        public Item Item { get; set; } = new Item();
        public int Quantity { get; set; }
    }
}
