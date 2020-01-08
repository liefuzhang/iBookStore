using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Infrastructure.EntityConfigurations
{
    public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.Property(ci => ci.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ci => ci.Price)
                .IsRequired();

            builder.Property(ci => ci.Author)
                .IsRequired();

            builder.HasOne(ci => ci.Category)
                .WithMany()
                .HasForeignKey(ci => ci.CategoryId);

            builder.Ignore(ci => ci.Rating);
        }
    }
}