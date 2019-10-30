using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;

namespace Ordering.Infrastructure
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderingContext context) {
            var domainEntities = context.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any());

            var domainEvents = domainEntities.SelectMany(e => e.Entity.DomainEvents).ToList();

            domainEntities.ToList().ForEach(e => e.Entity.ClearDomainEvents());

            var tasks = domainEvents.Select(async (domainEvent) => {
                await mediator.Publish(domainEvent);
            });

            await Task.WhenAll(tasks);
        }
    }
}
