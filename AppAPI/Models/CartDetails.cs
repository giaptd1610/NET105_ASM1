namespace AppAPI.Models
{
    public class CartDetails
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Username { get; set; }
        public int Quantity { get; set; }
        public int Satus { get; set; }
    }
}
