using L1.Core.Base.Event;
using L2.Application.Ports.Messaging;
using L3.Infrastructure.Persistence;
using MassTransit;

namespace L3.Worker.Adapters.Notification;

public class MassTransitEventDispatcher(
  AppDbContext dbContext,
  IPublishEndpoint publishEndpoint
) : IEventDispatcher {
  public async Task DispatchEventsAsync(CancellationToken ct = default) {
    var domainEntities = dbContext.ChangeTracker
      .Entries<IHasDomainEvent>()
      .Where(x => x.Entity.DomainEvents.Count != 0)
      .Select(x => x.Entity)
      .ToList();

    if (domainEntities.Count == 0) {
      return;
    }

    var domainEvents = domainEntities
      .SelectMany(x => x.DomainEvents)
      .ToList();

    foreach (var domainEvent in domainEvents) {
      await publishEndpoint.Publish((object)domainEvent, ct);
    }

    domainEntities.ForEach(x => x.ClearEvents());
  }
}
