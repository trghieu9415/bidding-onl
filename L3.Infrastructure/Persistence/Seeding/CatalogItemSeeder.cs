using Bogus;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Seeding;

public class CatalogItemSeeder(AppDbContext context) : ISeeder {
  public int Order => 3;

  public async Task SeedAsync() {
    if (await context.CatalogItems.AnyAsync()) {
      return;
    }

    var faker = new Faker("vi");
    var bidders = await context.Users.Where(u => u.Role == UserRole.Bidder).ToListAsync();
    var subCategories = await context.Categories.Where(c => c.ParentId != null).ToListAsync();

    foreach (var user in bidders.Take(10)) {
      var itemsCount = faker.Random.Int(3, 5);

      for (var i = 0; i < itemsCount; i++) {
        var productName = faker.Commerce.ProductName();
        var item = CatalogItem.Create(user.Id, productName, faker.Commerce.ProductDescription());

        item.SetStartingPrice(faker.Random.Int(50, 1000))
          .SetCondition(faker.PickRandom<ItemCondition>())
          .SyncCategories(new List<Guid> { faker.PickRandom(subCategories).Id });

        var mainImg = $"https://picsum.photos/seed/{Guid.NewGuid()}/800/600";
        var subImgs = Enumerable.Range(0, 3).Select(_ => $"https://picsum.photos/seed/{Guid.NewGuid()}/800/600")
          .ToList();

        item.SetImageUrls(mainImg, subImgs);
        item.Approve();

        context.CatalogItems.Add(item);
      }
    }

    await context.SaveChangesAsync();
  }
}
