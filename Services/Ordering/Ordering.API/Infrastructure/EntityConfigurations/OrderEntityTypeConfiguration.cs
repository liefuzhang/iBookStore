using Ordering.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.API.Infrastructure.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder) {
            builder.HasMany(o => o.OrderItems)
                 .WithOne()
                 .HasForeignKey(o => o.OrderId);

            //Address value object persisted as owned entity type supported since EF Core 2.0
            builder.OwnsOne(o => o.Address);
        }
    }
}