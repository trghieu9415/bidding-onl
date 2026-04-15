using L1.Core.Domain.Catalog.Entities;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace L3.Worker.Jobs;

public class ImageCleanupJob(AppDbContext dbContext) {
  public async Task Execute() {
    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "catalog");
    if (!Directory.Exists(uploadPath)) {
      return;
    }

    var dbImages = await dbContext.Set<CatalogItem>()
      .AsNoTracking()
      .Select(x => new { x.Images.MainImageUrl, x.Images.SubImageUrls })
      .ToListAsync();

    var activeImageUrls = dbImages
      .Select(x => x.MainImageUrl)
      .Concat(dbImages.SelectMany(x => x.SubImageUrls))
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
  }
}
