using L1.Core.Base.Event;
using L2.Application.Abstractions;
using L3.Infrastructure.Persistence;
using MassTransit;
using MediatR;

namespace L3.Infrastructure.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(
  IUnitOfWork unitOfWork,
  AppDbContext dbContext,
  IPublishEndpoint publishEndpoint
) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : ITransactional {
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) {
    await unitOfWork.BeginTransactionAsync(ct);

    try {
      var response = await next();

      var domainEntities = dbContext.ChangeTracker
        .Entries<IHasDomainEvent>()
        .Where(x => x.Entity.DomainEvents.Any())
        .Select(x => x.Entity)
        .ToList();

      var domainEvents = domainEntities
        .SelectMany(x => x.DomainEvents)
        .ToList();

      domainEntities.ForEach(x => x.ClearEvents());

      foreach (var domainEvent in domainEvents) {
        await publishEndpoint.Publish(domainEvent, ct);
      }

      await unitOfWork.SaveChangesAsync(ct);

      await unitOfWork.CommitTransactionAsync(ct);

      return response;
    } catch (Exception) {
      await unitOfWork.RollbackTransactionAsync(ct);
      throw;
    }
  }
}
