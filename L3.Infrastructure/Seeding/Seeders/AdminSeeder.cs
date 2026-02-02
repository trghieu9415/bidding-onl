using L2.Application.Models;
using L3.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Seeding.Seeders;

public class AdminSeeder(UserManager<AppUser> userManager) : ISeeder {
  public int Order => 1;

  public async Task SeedAsync() {
    if (!await userManager.Users.AnyAsync(u => u.Role == UserRole.Admin)) {
      var admin = new AppUser {
        Id = Guid.NewGuid(),
        UserName = "admin@bidding.com",
        Email = "admin@bidding.com",
        FullName = "System Admin",
        Role = UserRole.Admin,
        EmailConfirmed = true
      };
      await userManager.CreateAsync(admin, "Admin@123");
    }
  }
}
