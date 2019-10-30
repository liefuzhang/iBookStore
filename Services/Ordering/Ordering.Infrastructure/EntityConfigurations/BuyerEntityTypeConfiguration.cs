using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;

namespace Ordering.Infrastructure.EntityConfigurations
{
    public class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder) {
            builder.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("buyerseq");

            builder.Property(b => b.IdentityGuid)
                 .HasMaxLength(200)
                 .IsRequired();

            builder.HasIndex("IdentityGuid")
              .IsUnique(true);

            builder.HasMany(b => b.PaymentMethods)
               .WithOne()
               .HasForeignKey("BuyerId")
               .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
