using Membership.API.Controllers;
using Membership.API.Infrastructure.EntityConfigurations;
using Membership.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Membership.API.Infrastructure
{
    public class MembershipContext : DbContext
    {
        public MembershipContext(DbContextOptions<MembershipContext> options) : base(options)
        {
        }

        public DbSet<MembershipType> MembershipTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MembershipTypeEntityTypeConfiguration());
        }
    }

    public class MembershipContextDesignFactory : IDesignTimeDbContextFactory<MembershipContext>
    {
        public MembershipContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MembershipContext>()
                .UseSqlServer("Server=.;Initial Catalog=iBookStore.Services.MembershipDb;Integrated Security=true");

            return new MembershipContext(optionsBuilder.Options);
        }
    }
}