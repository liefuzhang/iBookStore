using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("orderseq");

            //Address value object persisted as owned entity type supported since EF Core 2.0
            builder.OwnsOne(o => o.Address);

            builder.Property<int?>("BuyerId").IsRequired(false);
            builder.Property<int?>("PaymentMethodId").IsRequired(false);

            builder.Property(o => o.Currency).HasDefaultValue("NZD");
            builder.Property(o => o.CurrencyRate).HasColumnType("decimal(18, 8)").HasDefaultValue(1);

            builder.HasOne<PaymentMethod>()
                .WithMany()
                .HasForeignKey("PaymentMethodId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Buyer>()
                .WithMany()
                .HasForeignKey("BuyerId")
                .IsRequired(false);
        }
    }
}