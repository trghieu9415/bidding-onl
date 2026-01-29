using System.ComponentModel.DataAnnotations;
using L2.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace L3.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid> {
  [MaxLength(100)] public string FullName { get; set; } = null!;
  [MaxLength(255)] public string? Url { get; set; }
  public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
  public DateTime? DeletedAt { get; private set; }
  public bool IsDeleted { get; private set; }

  public UserRole Role { get; init; } = UserRole.Bidder;

  public void Update(string fullName, string? url) {
    FullName = fullName;
    Url = url;
  }

  public void Delete() {
    IsDeleted = true;
    DeletedAt = DateTime.UtcNow;
  }

  public void Restore() {
    IsDeleted = false;
    DeletedAt = null;
  }
}
