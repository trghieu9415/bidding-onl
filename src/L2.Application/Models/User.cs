namespace L2.Application.Models;

public record User {
  public Guid Id { get; init; } = Guid.NewGuid();
  public required string FullName { get; init; }
  public required string Email { get; init; }
  public string? PhoneNumber { get; init; }
  public string? Url { get; init; }
  public bool IsActive { get; init; }
  public UserRole Role { get; init; }
  public string? SecurityStamp { get; init; }
}

public enum UserRole { Admin, Bidder }
