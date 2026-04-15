using L1.Core.Domain.Catalog.Entities;
using L3.Infrastructure.Persistence;
using L3.Infrastructure.Persistence.Identity;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Services;

public class ImageTracker(AppDbContext dbContext) : IImageTracker {
  public async Task<HashSet<string>> GetInUseImageUrlsAsync(CancellationToken ct = default) {
    var auctionUrls = await dbContext.Set<CatalogItem>()
      .Where(x => !x.IsDeleted && x.Images.MainImageUrl != null)
      .Where(x => !string.IsNullOrEmpty(x.Images.MainImageUrl))
      .SelectMany(x =>
        x.Images.SubImageUrls.Concat(new[] { x.Images.MainImageUrl })
      )
      .Distinct()
      .ToListAsync(ct);


    var userUrls = await dbContext.Set<AppUser>()
      .Where(x => !x.IsDeleted && !string.IsNullOrEmpty(x.Url))
      .Select(x => x.Url!)
      .ToListAsync(ct);

    var urls = auctionUrls.Concat(userUrls);
    return [..urls!];
  }
}
