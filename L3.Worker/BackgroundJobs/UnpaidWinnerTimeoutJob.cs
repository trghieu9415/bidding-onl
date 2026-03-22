using Hangfire;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Catalog.Entities;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace L3.Worker.BackgroundJobs;

[DisableConcurrentExecution(300)]
public class UnpaidWinnerTimeoutJob(AppDbContext dbContext) {
  public async Task Execute(CancellationToken ct) {
    var deadline = DateTime.UtcNow.AddDays(-1);

    var expiredAuctions = await dbContext.Set<Auction>()
      .Where(a =>
        a.Status == AuctionStatus.EndedSold &&
        a.WinningAt > deadline
      ).ToListAsync(ct);

    foreach (var auction in expiredAuctions) {
      var item = await dbContext.Set<CatalogItem>()
        .FirstOrDefaultAsync(i => i.Id == auction.CatalogItemId, ct);
      item?.Sell(false);
      auction.Paid(false);
    }


    await dbContext.SaveChangesAsync(ct);
  }
}
