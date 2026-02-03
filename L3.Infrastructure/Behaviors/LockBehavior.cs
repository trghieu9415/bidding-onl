using L2.Application.Abstractions;
using L2.Application.Exceptions;
using L2.Application.Ports.Concurrency;
using MediatR;

namespace L3.Infrastructure.Behaviors;

public class LockBehavior<TRequest, TResponse>(
  IDistributedLockService lockService
  ) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : ILockable {
  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken ct
  ) {
    using var distributedLock = await lockService.AcquireLockAsync(
      request.LockKey,
      request.Expiration,
      request.WaitTime
    );

    if (distributedLock == null) {
      throw new AppException($"Hệ thống đang bận xử lý yêu cầu này (Key: {request.LockKey}). Vui lòng thử lại.", 429);
    }

    return await next();
  }
}
