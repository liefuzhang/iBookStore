using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure.EntityConfigurations
{
    public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder) {
            builder.Property<int>("BuyerId").IsRequired();

            builder.Property<string>("CardHolderName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("CardNumber")
                .HasMaxLength(25)
                .IsRequired();

            builder.Property<DateTime>("Expiration")
                .IsRequired();
        }
    }
}
