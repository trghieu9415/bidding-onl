using L3.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence;

public class DbInitializer(AppDbContext context, IEnumerable<ISeeder> seeders) {
  public async Task SeedAsync() {
    await context.Database.MigrateAsync();

    foreach (var seeder in seeders.OrderBy(s => s.Order)) {
      await seeder.SeedAsync();
    }

    Console.WriteLine("---- Seeding completed successfully! ----");
  }
}