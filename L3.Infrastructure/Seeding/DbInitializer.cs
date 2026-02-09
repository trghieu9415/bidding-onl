using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace L3.Infrastructure.Seeding;

public class DbInitializer(
  AppDbContext context,
  IEnumerable<ISeeder> seeders,
  IConfiguration configuration
) {
  public async Task SeedAsync() {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    await EnsureQuartzTablesResourceCreated(connectionString!);
    await context.Database.MigrateAsync();

    foreach (var seeder in seeders.OrderBy(s => s.Order)) {
      await seeder.SeedAsync();
    }

    Console.WriteLine("---- Seeding completed successfully! ----");
  }

  private static async Task EnsureQuartzTablesResourceCreated(string connectionString) {
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    const string checkTableExists =
      """
      SELECT EXISTS (
          SELECT 1
          FROM information_schema.tables
          WHERE table_schema = 'public'
            AND table_name = 'qrtz_job_details'
      );
      """;
    await using var checkCommand = new NpgsqlCommand(checkTableExists, connection);
    var exists = (bool?)await checkCommand.ExecuteScalarAsync() ?? false;

    if (!exists) {
      Console.WriteLine("[+] Quartz tables not found. Initializing schema...");
      var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Scripts", "tables_postgres.sql");
      var script = await File.ReadAllTextAsync(path);

      await using var command = new NpgsqlCommand(script, connection);
      await command.ExecuteNonQueryAsync();
      Console.WriteLine("[+] Quartz tables created successfully.");
    }
  }
}
