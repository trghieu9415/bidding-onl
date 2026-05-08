namespace L2.Application.Abstractions;

public interface IUnitOfWork {
  Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken ct = default);
  Task<int> SaveChangesAsync(CancellationToken ct = default);
  Task CommitTransactionAsync(CancellationToken ct = default);
}
