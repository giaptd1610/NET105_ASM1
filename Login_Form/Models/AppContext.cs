using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Login_Form.Models
{
    public class AppContext : DbContext
    {
        public AppContext() { }
        public AppContext(DbContextOptions options) : base(options)
        {

        }
        //Khai báo các DbSet
        public DbSet<Account>  Accounts { get; set; }  //Mỗi DbSet tượng trưng cho 1 bảng trong sql
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=MSI\\MSI;Initial Catalog=Review_Net104;Integrated Security=True; TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        // thực hiện override các phương thức cấu hình


    }
}
