using Bogus;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Seeding;

public class AuctionSessionSeeder(AppDbContext context) : ISeeder {
  public int Order => 4;

  public async Task SeedAsync() {
    if (await context.AuctionSessions.AnyAsync()) {
      return;
    }

    var faker = new Faker("vi");
    var approvedItems = await context.CatalogItems.Where(x => x.Status == ItemStatus.Approval).ToListAsync();
    var bidders = await context.Users.Where(u => u.Role == UserRole.Bidder).ToListAsync();

    var liveSession = AuctionSession.Create("Đại tiệc Đấu giá Cuối tuần", DateTime.Now.AddHours(-2),
      DateTime.Now.AddDays(1));
    context.AuctionSessions.Add(liveSession);
    await context.SaveChangesAsync();

    var auctionIds = new List<Guid>();
    foreach (var item in approvedItems.Take(15)) {
      var auction = Auction.Create(item.Id, item.StartingPrice, 10, item.StartingPrice + 50)
        .SetOwnerId(item.OwnerId);

      auction.Start();

      var bidCount = faker.Random.Int(3, 7);
      var currentPrice = item.StartingPrice;
      for (var j = 0; j < bidCount; j++) {
        currentPrice += 20;
        var bidder = faker.PickRandom(bidders);
        if (bidder.Id != item.OwnerId) {
          auction.PlaceBid(bidder.Id, currentPrice);
        }
      }

      context.Auctions.Add(auction);
      auctionIds.Add(auction.Id);
    }

    liveSession.SyncAuctions(auctionIds);
    liveSession.Start();

    await context.SaveChangesAsync();
  }
}
