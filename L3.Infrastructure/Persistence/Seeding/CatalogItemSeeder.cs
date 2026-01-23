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

    var owner = await context.Users.FirstAsync(u => u.Role == UserRole.Bidder);
    var category = await context.Categories.FirstOrDefaultAsync();

    var items = new List<CatalogItem> {
      CatalogItem.Create(owner.Id, "iPhone 15 Pro Max 512GB", "Hàng mới 99% nguyên zin")
        .SetStartingPrice(25000000)
        .SetCondition(ItemCondition.UsedGood),

      CatalogItem.Create(owner.Id, "Macbook M3 Pro 2024", "Fullbox chưa qua sửa chữa")
        .SetStartingPrice(45000000)
        .SetCondition(ItemCondition.NewSealed)
    };

    foreach (var item in items) {
      if (category != null) {
        item.SyncCategories(new List<Guid> { category.Id });
      }

      item.Approve();
      context.CatalogItems.Add(item);
    }

    await context.SaveChangesAsync();
  }
}
