using L1.Core.Base.Event;
using L2.Application.Abstractions;
using L3.Infrastructure.Persistence;
using MassTransit;
using MediatR;

namespace L3.Infrastructure.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(
  AppDbContext dbContext,
  IPublishEndpoint publishEndpoint
) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : ITransactional {
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) {
    await dbContext.BeginTransactionAsync(ct);

    try {
      var response = await next();

      var domainEntities = dbContext.ChangeTracker
        .Entries<IHasDomainEvent>()
        .Where(x => x.Entity.DomainEvents.Count != 0)
        .Select(x => x.Entity)
        .ToList();

      var domainEvents = domainEntities
        .SelectMany(x => x.DomainEvents)
        .ToList();

      Console.WriteLine($"Domain Events: {domainEvents.Count}");

      domainEntities.ForEach(x => x.ClearEvents());

      await Task.WhenAll(domainEvents.Select(evt => publishEndpoint.Publish((object)evt, ct)));

      await dbContext.SaveChangesAsync(ct);
      await dbContext.CommitTransactionAsync(ct);
      return response;
    } catch (Exception) {
      await dbContext.RollbackTransactionAsync(ct);
      throw;
    }
  }
}
