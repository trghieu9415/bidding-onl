using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Catalog.Entities;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace L3.Worker.BackgroundJobs;

[DisallowConcurrentExecution]
public class UnpaidWinnerTimeoutJob(AppDbContext dbContext) : IJob {
  public async Task Execute(IJobExecutionContext context) {
    var deadline = DateTime.UtcNow.AddDays(-1);

    var expiredAuctions = await dbContext.Set<Auction>()
      .Where(a =>
        a.Status == AuctionStatus.EndedSold &&
        a.WinningAt > deadline
      ).ToListAsync();

    foreach (var auction in expiredAuctions) {
      var item = await dbContext.Set<CatalogItem>()
        .FirstOrDefaultAsync(i => i.Id == auction.CatalogItemId);
      item?.Sell(false);
      auction.Paid(false);
    }


    await dbContext.SaveChangesAsync();
  }
}
