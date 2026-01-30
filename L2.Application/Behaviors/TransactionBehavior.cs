using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
  : IPipelineBehavior<TRequest, TResponse>
  where TRequest : ITransactional {
  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken ct
  ) {
    await unitOfWork.BeginTransactionAsync(ct);
    try {
      var response = await next();

      await unitOfWork.SaveChangesAsync(ct);
      await unitOfWork.CommitTransactionAsync(ct);
      return response;
    } catch (Exception) {
      await unitOfWork.RollbackTransactionAsync(ct);
      throw;
    }
  }
}
