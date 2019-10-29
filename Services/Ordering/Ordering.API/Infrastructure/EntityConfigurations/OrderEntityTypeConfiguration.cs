﻿using Ordering.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate;

namespace Ordering.API.Infrastructure.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder) {
            builder.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("orderseq");

            //Address value object persisted as owned entity type supported since EF Core 2.0
            builder.OwnsOne(o => o.Address);

            builder.Property<int?>("BuyerId").IsRequired(false);
            builder.Property<int?>("PaymentMethodId").IsRequired(false);

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