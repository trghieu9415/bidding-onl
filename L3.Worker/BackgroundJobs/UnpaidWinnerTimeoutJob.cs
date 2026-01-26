// FILE: L3.Infrastructure/BackgroundJobs/UnpaidWinnerTimeoutJob.cs

using L1.Core.Domain.Bidding.Enums;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace L3.Worker.BackgroundJobs;

public class UnpaidWinnerTimeoutJob(AppDbContext dbContext) : IJob {
  public async Task Execute(IJobExecutionContext context) {
    var deadline = DateTime.Now.AddDays(-1);

    var expiredAuctions = await dbContext.Auctions
      .Where(a => a.Status == AuctionStatus.EndedSold)
      .ToListAsync();

    foreach (var auction in expiredAuctions) {
      var item = await dbContext.CatalogItems.FirstOrDefaultAsync(i => i.Id == auction.CatalogItemId);
      item?.Sell(false);
    }

    // auction.MarkAsPaymentFailed(); // Cần định nghĩa thêm trong Entity

    await dbContext.SaveChangesAsync();
  }
}
