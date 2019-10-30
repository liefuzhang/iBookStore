using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;

namespace Ordering.Infrastructure.EntityConfigurations
{
    public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder) {
            builder.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("paymentmethodseq");

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
