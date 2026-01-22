namespace L3.Infrastructure.Persistence.Seeding;

public interface ISeeder {
  int Order { get; }
  Task SeedAsync();
}