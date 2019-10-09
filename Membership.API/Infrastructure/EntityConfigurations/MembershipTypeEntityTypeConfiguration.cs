using Membership.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Membership.API.Infrastructure.EntityConfigurations
{
    public class MembershipTypeEntityTypeConfiguration : IEntityTypeConfiguration<MembershipType>
    {
        public void Configure(EntityTypeBuilder<MembershipType> builder)
        {
            builder.ToTable("MembershipType");

            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.Name)
                .IsRequired();
        }
    }
}