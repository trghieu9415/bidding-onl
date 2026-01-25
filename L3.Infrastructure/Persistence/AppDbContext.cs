using System.Data;
using L1.Core.Base.Event;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Ports.Repository;
using L3.Infrastructure.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace L3.Infrastructure.Persistence;

public class AppDbContext(
  DbContextOptions<AppDbContext> options,
  IPublishEndpoint publishEndpoint
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
      await base.SaveChangesAsync(ct);

      if (_currentTransaction != null) {
        await _currentTransaction.CommitAsync(ct);
      }
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

  public override async Task<int> SaveChangesAsync(CancellationToken ct = default) {
    var entitiesWithEvents = ChangeTracker.Entries<IHasDomainEvent>()
      .Select(x => x.Entity)
      .Where(x => x.DomainEvents.Count != 0)
      .ToList();

    var domainEvents = entitiesWithEvents.SelectMany(x => x.DomainEvents).ToList();
    entitiesWithEvents.ForEach(x => x.ClearEvents());

    foreach (var domainEvent in domainEvents) {
      await publishEndpoint.Publish(domainEvent, ct);
    }

    return await base.SaveChangesAsync(ct);
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
