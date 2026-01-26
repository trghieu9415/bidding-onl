using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Enums;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Seeding;

public class AuctionSessionSeeder(AppDbContext context) : ISeeder {
  public int Order => 4;

  public async Task SeedAsync() {
    if (await context.AuctionSessions.AnyAsync()) {
      return;
    }

    var items = await context.CatalogItems
      .Where(x => x.Status == ItemStatus.Approval)
      .ToListAsync();

    if (items.Count == 0) {
      return;
    }

    var auctions = items.Select(item =>
      Auction.Create(item.Id, item.StartingPrice, 500000, item.StartingPrice + 2000000).SetOwnerId(item.OwnerId)
    ).ToList();

    context.Auctions.AddRange(auctions);
    await context.SaveChangesAsync();

    var session = AuctionSession.Create(
      "Phiên đấu giá đồ công nghệ tháng 1/2026",
      DateTime.Now.AddDays(1),
      DateTime.Now.AddDays(2)
    );

    session.SyncAuctions(auctions.Select(a => a.Id).ToList());
    session.Publish();

    context.AuctionSessions.Add(session);
    await context.SaveChangesAsync();
  }
}
