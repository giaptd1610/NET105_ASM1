using Login_Form.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Login_Form.Configuration
{
    public class BillConfig : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(p => p.Id);
            //Khoá ngoại
            builder.HasOne(p => p.Account).WithMany(p => p.Bills).
                HasForeignKey(p => p.Username);
            //
            //builder.HasAlternateKey(); //Set thuộc tính unique
        }
    }
}
