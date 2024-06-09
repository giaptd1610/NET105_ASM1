namespace Login_Form.Models
{
    public class CartDetail
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Username { get; set; }
        public int Quantity { get; set; }
        public int Satus { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}
