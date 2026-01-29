using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace L3.Worker.BackgroundJobs;

public class SoftDeletePurgeJob(AppDbContext dbContext) : IJob {
  public async Task Execute(IJobExecutionContext context) {
    var threshold = DateTime.UtcNow.AddDays(-30);

    var oldCategories = await dbContext.Categories
      .Where(x => x.IsDeleted && x.DeletedAt < threshold)
      .ToListAsync();
    dbContext.Categories.RemoveRange(oldCategories);

    var oldItems = await dbContext.CatalogItems
      .Where(x => x.IsDeleted && x.DeletedAt < threshold)
      .ToListAsync();
    dbContext.CatalogItems.RemoveRange(oldItems);

    await dbContext.SaveChangesAsync();
  }
}
