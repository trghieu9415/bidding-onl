using L1.Core.Base.Event;
using L2.Application.Ports.Gateways;
using L3.Infrastructure.Persistence;
using MassTransit;

namespace L3.Infrastructure.Adapters.Gateways;

public class MassTransitEventDispatcher(
  AppDbContext dbContext,
  IPublishEndpoint publishEndpoint
) : IEventDispatcher {
  public async Task DispatchEventsAsync(CancellationToken ct = default) {
    var domainEntities = dbContext.ChangeTracker
      .Entries<IHasDomainEvent>()
      .Where(x => x.Entity.DomainEvents.Any())
      .Select(x => x.Entity)
      .ToList();

    if (domainEntities.Count == 0) {
      return;
    }

    var domainEvents = domainEntities
      .SelectMany(x => x.DomainEvents)
      .ToList();

    domainEntities.ForEach(x => x.ClearEvents());

    await Task.WhenAll(domainEvents.Select(evt => publishEndpoint.Publish((object)evt, ct)));
  }
}
