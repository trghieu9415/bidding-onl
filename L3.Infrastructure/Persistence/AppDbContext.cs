using System.Data;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Abstractions;
using L2.Application.Exceptions;
using L3.Infrastructure.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace L3.Infrastructure.Persistence;

public class AppDbContext(
  DbContextOptions<AppDbContext> options
) : IdentityUserContext<AppUser, Guid>(options), IUnitOfWork {
  private IDbContextTransaction? _currentTransaction;
  public DbSet<Auction> Auctions => Set<Auction>();
  public DbSet<AuctionSession> AuctionSessions => Set<AuctionSession>();
  public DbSet<Bid> Bids => Set<Bid>();
  public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
  public DbSet<Category> Categories => Set<Category>();


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
    } catch (DbUpdateConcurrencyException) {
      await RollbackTransactionAsync(ct);
      throw new AppException("Dữ liệu đã bị thay đổi. Vui lòng thử lại.", 409);
    } catch {
      await RollbackTransactionAsync(ct);
      throw;
    } finally {
      if (_currentTransaction != null) {
        _currentTransaction.Dispose();
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
        _currentTransaction.Dispose();
        _currentTransaction = null;
      }
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.HasPostgresExtension("pg_trgm");
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<AppUser>()
      .Property(u => u.Role)
      .HasConversion<string>();

    // NOTE: ========== [MassTransit Outbox Entities] ==========
    modelBuilder.AddInboxStateEntity();
    modelBuilder.AddOutboxMessageEntity();
    modelBuilder.AddOutboxStateEntity();

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }
}
