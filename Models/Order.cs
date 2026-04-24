namespace RentalService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
