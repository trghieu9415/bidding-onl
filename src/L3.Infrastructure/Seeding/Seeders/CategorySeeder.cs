using L1.Core.Domain.Catalog.Entities;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Seeding.Seeders;

public class CategorySeeder(AppDbContext context) : ISeeder {
  public int Order => 2;

  public async Task SeedAsync() {
    if (await context.Set<Category>().AnyAsync()) {
      return;
    }

    var categories = new Dictionary<string, string[]> {
      ["Điện tử"] = ["Điện thoại", "Laptop", "Máy ảnh", "Loa Bluetooth"],
      ["Thời trang"] = ["Đồng hồ", "Túi xách", "Giày Sneaker", "Trang sức"],
      ["Đồ sưu tầm"] = ["Tiền cổ", "Tem", "Mô hình Figure", "Thẻ bài Pokemon"],
      ["Gia dụng"] = ["Máy pha cà phê", "Robot hút bụi", "Nồi chiên không dầu"]
    };

    foreach (var parent in categories) {
      var parentCat = Category.Create(parent.Key);
      context.Set<Category>().Add(parentCat);
      await context.SaveChangesAsync();

      foreach (var childName in parent.Value) {
        context.Set<Category>().Add(Category.Create(childName, parentCat.Id));
      }
    }

    await context.SaveChangesAsync();
  }
}
