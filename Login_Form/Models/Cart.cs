namespace Login_Form.Models
{
    public class Cart
    {
        public string Username { get; set; }
        public int Status { get; set; }
        public virtual List<CartDetail> Details { get; set; }
        public Account Account { get; set; }
    }
}
