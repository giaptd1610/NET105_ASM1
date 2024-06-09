using Login_Form.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Login_Form.Configuration
{
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            //Khoá chính
            builder.HasKey(p => p.Username);
            // Key có nhiều cột
            //builder.HasKey(p => new { p.Username, p.Password });
            //builder.HasNoKey(); -> Không có khoá chính
            builder.Property(p => p.Password).HasColumnType("varchar(256)");
            builder.Property(p => p.Address).IsUnicode(true).IsFixedLength(true).HasMaxLength(256);
        }
    }
}
