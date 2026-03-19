using L1.Core.Domain.Catalog.Entities;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace L3.Worker.BackgroundJobs;

public class SoftDeletePurgeJob(AppDbContext dbContext) : IJob {
  public async Task Execute(IJobExecutionContext context) {
    var threshold = DateTime.UtcNow.AddDays(-30);

    var oldCategories = await dbContext.Set<Category>()
      .Where(x => x.IsDeleted && x.DeletedAt < threshold)
      .ToListAsync();
    dbContext.Set<Category>().RemoveRange(oldCategories);

    var oldItems = await dbContext.Set<CatalogItem>()
      .Where(x => x.IsDeleted && x.DeletedAt < threshold)
      .ToListAsync();
    dbContext.Set<CatalogItem>().RemoveRange(oldItems);

    await dbContext.SaveChangesAsync();
  }
}
