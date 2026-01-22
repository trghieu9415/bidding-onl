using System.Security.Claims;
using L2.Application.Models;
using L2.Application.Ports.Security;
using Microsoft.AspNetCore.Http;

namespace L3.Infrastructure.Adapters.Security;

public class CurrentUser : ICurrentUser {
  public CurrentUser(IHttpContextAccessor accessor) {
    var user = accessor.HttpContext?.User;
    var id = user?.FindFirstValue(ClaimTypes.NameIdentifier);

    if (id != null) {
      User = new User {
        Id = Guid.Parse(id),
        Email = user?.FindFirstValue(ClaimTypes.Email) ?? "",
        FullName = user?.Identity?.Name ?? "",
        Role = user?.FindFirstValue(ClaimTypes.Role) == nameof(UserRole.Admin) ? UserRole.Admin : UserRole.Bidder
      };
    } else {
      User = new User { FullName = "Guest" };
    }
  }

  public User User { get; init; }
}