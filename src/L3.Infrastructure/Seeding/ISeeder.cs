namespace L3.Infrastructure.Seeding;

public interface ISeeder {
  int Order { get; }
  Task SeedAsync();
}
