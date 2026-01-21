using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence;

public class DbInitializer(
  AppDbContext context
) {
  public async Task SeedAsync() {
    await context.Database.MigrateAsync();

    Console.WriteLine("---- Seeding completed successfully! ----");
  }
}
