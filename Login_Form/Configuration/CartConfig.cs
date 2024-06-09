using Login_Form.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Login_Form.Configuration
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(p=>p.Username);
            //cấu hình 1 1
            builder.HasOne(p => p.Account).WithOne(p => p.Cart).
                HasForeignKey<Cart>(p=>p.Username);
        }
    }
}
