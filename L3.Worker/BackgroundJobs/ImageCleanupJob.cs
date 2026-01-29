using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace L3.Worker.BackgroundJobs;

public class ImageCleanupJob(AppDbContext dbContext) : IJob {
  public async Task Execute(IJobExecutionContext context) {
    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "catalog");
    if (!Directory.Exists(uploadPath)) {
      return;
    }

    var dbImages = await dbContext.CatalogItems
      .AsNoTracking()
      .Select(x => new { x.Images.MainImageUrl, x.Images.SubImageUrls })
      .ToListAsync();

    var activeImageUrls = dbImages
      .Select(x => x.MainImageUrl)
      .Concat(dbImages.SelectMany(x => x.SubImageUrls ?? new List<string>()))
      .Where(url => !string.IsNullOrEmpty(url))
      .ToHashSet();

    var files = Directory.GetFiles(uploadPath);
    foreach (var filePath in files) {
      var fileName = Path.GetFileName(filePath);
      var publicUrl = $"/uploads/catalog/{fileName}";

      if (activeImageUrls.Contains(publicUrl)) {
        continue;
      }

      var creationTime = File.GetCreationTime(filePath);
      if (creationTime < DateTime.UtcNow.AddDays(-1)) {
        File.Delete(filePath);
      }
    }

    await Task.CompletedTask;
  }
}
