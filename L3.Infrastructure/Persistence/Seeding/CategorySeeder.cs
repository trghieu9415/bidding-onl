using L1.Core.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Seeding;

public class CategorySeeder(AppDbContext context) : ISeeder {
  public int Order => 2;

  public async Task SeedAsync() {
    if (!await context.Categories.AnyAsync()) {
      var electronics = Category.Create("Điện tử");
      var fashion = Category.Create("Thời trang");

      context.Categories.AddRange(electronics, fashion);
      await context.SaveChangesAsync();

      var laptop = Category.Create("Laptop", electronics.Id);
      context.Categories.Add(laptop);
      await context.SaveChangesAsync();
    }
  }
}