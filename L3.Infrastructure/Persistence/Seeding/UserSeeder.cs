using L2.Application.Models;
using L3.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Seeding;

public class UserSeeder(UserManager<AppUser> userManager) : ISeeder {
  public int Order => 1;

  public async Task SeedAsync() {
    if (!await userManager.Users.AnyAsync(u => u.Role == UserRole.Bidder)) {
      var bidder = new AppUser {
        UserName = "bidder1@gmail.com", Email = "bidder1@gmail.com",
        FullName = "Nguyễn Văn Đấu Giá", Role = UserRole.Bidder, EmailConfirmed = true
      };
      await userManager.CreateAsync(bidder, "Bidder@123");
    }
  }
}
