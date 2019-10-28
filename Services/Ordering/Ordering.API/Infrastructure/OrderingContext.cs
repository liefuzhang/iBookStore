using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Ordering.API.Infrastructure.EntityConfigurations;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure
{
    public class OrderingContext : DbContext
    {
        private readonly IMediator _mediator;

        private OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {
        }

        public OrderingContext(DbContextOptions<OrderingContext> options, IMediator mediator) : base(options) {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Buyer> Buyers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            builder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
            builder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync();

            return true;
        }
    }

    public class OrderingContextDesignFactory : IDesignTimeDbContextFactory<OrderingContext>
    {
        public OrderingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>()
                .UseSqlServer("Server=.;Initial Catalog=iBookStore.Services.OrderingDb;Integrated Security=true");

            return new OrderingContext(optionsBuilder.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification {
                return Task.CompletedTask;
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default) {
                throw new NotImplementedException();
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken)) {
                return Task.FromResult<TResponse>(default(TResponse));
            }

            public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken)) {
                return Task.CompletedTask;
            }
        }
    }
}