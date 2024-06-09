namespace Login_Form.Models
{
    public class BillDetail // chứa thông tin của 1 sản phẩm trong hoá đơn
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string BillId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } //Giá tại thời điểm mua hoặc bán
        public virtual Bill Bill { get; set; }
        public virtual Product Product { get; set; }

    }
}
