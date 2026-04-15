using Bogus;
using L2.Application.Models;
using L3.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Seeding.Seeders;

public class UserSeeder(UserManager<AppUser> userManager) : ISeeder {
  public int Order => 1;

  public async Task SeedAsync() {
    if (await userManager.Users.AnyAsync(u => u.Role == UserRole.Bidder)) {
      return;
    }

    var faker = new Faker("vi");
    var users = new List<AppUser>();

    for (var i = 0; i < 20; i++) {
      var firstName = faker.Name.FirstName();
      var lastName = faker.Name.LastName();
      var fullName = $"{lastName} {firstName}";
      var email = $"user.num{i}@gmail.com";

      var user = new AppUser {
        Id = Guid.NewGuid(),
        UserName = email,
        Email = email,
        FullName = fullName,
        PhoneNumber = faker.Phone.PhoneNumber("09########"),
        Url = $"https://api.dicebear.com/9.x/avataaars/svg?seed={email}",
        Role = UserRole.Bidder,
        EmailConfirmed = true
      };
      users.Add(user);
    }

    foreach (var u in users) {
      await userManager.CreateAsync(u, "111qqq...");
    }
  }
}
