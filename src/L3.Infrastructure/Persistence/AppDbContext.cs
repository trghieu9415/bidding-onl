using System.Data;
using L2.Application.Abstractions;
using L3.Infrastructure.Persistence.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace L3.Infrastructure.Persistence;

public class AppDbContext(
  DbContextOptions<AppDbContext> options
) : IdentityUserContext<AppUser, Guid>(options), IUnitOfWork {
  private IDbContextTransaction? _currentTransaction;

  public async Task BeginTransactionAsync(CancellationToken ct = default) {
    if (_currentTransaction != null) {
      return;
    }

    _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
  }

  public async Task CommitTransactionAsync(CancellationToken ct = default) {
    try {
      if (_currentTransaction != null) {
        await _currentTransaction.CommitAsync(ct);
      }
    } catch {
      await RollbackTransactionAsync(ct);
      throw;
    } finally {
      if (_currentTransaction != null) {
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
      }
    }
  }

  public async Task RollbackTransactionAsync(CancellationToken ct = default) {
    try {
      if (_currentTransaction != null) {
        await _currentTransaction.RollbackAsync(ct);
      }
    } finally {
      if (_currentTransaction != null) {
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
      }
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.HasPostgresExtension("pg_trgm");
    base.OnModelCreating(modelBuilder);

    // NOTE: ========== [MassTransit Outbox Entities] ==========
    modelBuilder.AddInboxStateEntity();
    modelBuilder.AddOutboxMessageEntity();
    modelBuilder.AddOutboxStateEntity();

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }
}
