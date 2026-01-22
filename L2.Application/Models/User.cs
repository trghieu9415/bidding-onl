namespace L2.Application.Models;

public record User {
  public Guid Id { get; init; } = Guid.NewGuid();
  public string FullName { get; init; } = null!;
  public string Email { get; init; } = null!;
  public string? PhoneNumber { get; init; }
  public string? Url { get; init; }
  public bool IsActive { get; init; } = true;
  public UserRole Role { get; init; }
}

public enum UserRole {
  Admin,
  Bidder
}