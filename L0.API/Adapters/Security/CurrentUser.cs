using System.Security.Claims;
using L2.Application.Models;
using L2.Application.Ports.Security;

namespace L0.API.Adapters.Security;

public class CurrentUser : ICurrentUser {
  public CurrentUser(IHttpContextAccessor accessor) {
    var user = accessor.HttpContext?.User;
    var id = user?.FindFirstValue(ClaimTypes.NameIdentifier);

    if (id != null) {
      Id = Guid.Parse(id);
      FullName = user?.Identity?.Name ?? "Guest";
      Email = user?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
      Role =
        user?.FindFirstValue(ClaimTypes.Role) ==
        nameof(UserRole.Admin)
          ? UserRole.Admin
          : UserRole.Bidder;
    } else {
      Id = Guid.Empty;
    }
  }

  public Guid Id { get; init; }
  public string FullName { get; init; } = "Guest";
  public string Email { get; init; } = string.Empty;
  public UserRole? Role { get; init; }
  public bool IsLoggedIn => Id != Guid.Empty;
}
