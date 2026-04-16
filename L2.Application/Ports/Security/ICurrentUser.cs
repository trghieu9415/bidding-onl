using L2.Application.Models;

namespace L2.Application.Ports.Security;

public interface ICurrentUser {
  Guid Id { get; init; }
  string FullName { get; init; }
  string Email { get; init; }
  public UserRole? Role { get; init; }
  public bool IsLoggedIn { get; }
}
