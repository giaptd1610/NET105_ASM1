namespace AppAPI.Models
{
    public class Bills
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string Username { get; set; }
        public int Status { get; set; }
    }
}
