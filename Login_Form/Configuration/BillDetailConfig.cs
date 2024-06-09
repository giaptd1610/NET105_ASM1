using Login_Form.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Login_Form.Configuration
{
    public class BillDetailConfig : IEntityTypeConfiguration<BillDetail>
    {
        public void Configure(EntityTypeBuilder<BillDetail> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Bill).WithMany(p => p.Details).
                HasForeignKey(p=>p.BillId);
            builder.HasOne(p => p.Product).WithMany(p=>p.BillDetails).
                HasForeignKey(p=>p.ProductId);

        }
    }
}
