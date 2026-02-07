using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Identity;
using L3.Infrastructure.Exceptions;
using L3.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace L3.Infrastructure.Adapters.Identity;

public class UserService(
  UserManager<AppUser> userManager,
  ISieveProcessor sieveProcessor
) : IUserService {
  public async Task<User?> GetByIdAsync(Guid id, UserRole role = UserRole.Bidder, CancellationToken ct = default) {
    var user = await userManager.Users
      .FirstOrDefaultAsync(u => u.Id == id && u.Role == role, ct);

    return user == null ? null : ToUser(user);
  }

  public async Task<(int total, List<User> users)> GetAsync(
    SieveModel? sieveModel = null,
    UserRole role = UserRole.Bidder,
    CancellationToken ct = default
  ) {
    var query = userManager.Users.AsNoTracking()
      .Where(u => u.Role == role && !u.IsDeleted);
    if (sieveModel != null) {
      query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
    }

    var total = await query.CountAsync(ct);
    if (sieveModel != null) {
      query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
    }

    var appUsers = await query.ToListAsync(ct);
    var users = appUsers.Select(ToUser).ToList();
    return (total, users);
  }

  public async Task<(int total, List<User> users)> GetDeletedAsync(
    SieveModel? sieveModel = null,
    UserRole role = UserRole.Bidder,
    CancellationToken ct = default
  ) {
    var query = userManager.Users.AsNoTracking()
      .Where(u => u.Role == role && u.IsDeleted);
    if (sieveModel != null) {
      query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
    }

    var total = await query.CountAsync(ct);
    if (sieveModel != null) {
      query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
    }

    var appUsers = await query.ToListAsync(ct);
    var users = appUsers.Select(ToUser).ToList();
    return (total, users);
  }

  public async Task<Guid> CreateAsync(
    User user,
    string password,
    UserRole role,
    CancellationToken ct = default
  ) {
    var appUser = new AppUser {
      Id = Guid.NewGuid(),
      UserName = user.Email,
      Email = user.Email,
      FullName = user.FullName,
      PhoneNumber = user.PhoneNumber,
      Url = user.Url,
      Role = role
    };
    await userManager.CreateAsync(appUser, password);
    return appUser.Id;
  }

  public async Task UpdateAsync(User user, CancellationToken ct = default) {
    var existingUser = await userManager.FindByIdAsync(user.Id.ToString());
    if (existingUser == null || existingUser.IsDeleted) {
      throw new AppException($"Người dùng không tồn tại - Id: {user.Id}.", 404);
    }

    existingUser.FullName = user.FullName;
    existingUser.PhoneNumber = user.PhoneNumber;
    existingUser.Url = user.Url;

    await userManager.UpdateAsync(existingUser);
  }

  public async Task DeleteAsync(Guid id, CancellationToken ct = default) {
    var user = await FindOrThrowAsync(id, ct);
    if (user.Role == UserRole.Admin) {
      throw new InfrastructureException("Không thể xóa người dùng quản trị");
    }

    user.Delete();
    await userManager.UpdateAsync(user);
  }

  public async Task RestoreAsync(Guid id, CancellationToken ct = default) {
    var user = await FindOrThrowAsync(id, ct);
    user.Restore();
    await userManager.UpdateAsync(user);
  }

  public async Task LockAsync(Guid id, CancellationToken ct = default) {
    var user = await FindOrThrowAsync(id, ct);
    await userManager.SetLockoutEnabledAsync(user, true);
    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
  }

  public async Task UnlockAsync(Guid id, CancellationToken ct = default) {
    var user = await FindOrThrowAsync(id, ct);
    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
  }

  // NOTE: ========== [Helper Methods] ==========
  private static User ToUser(AppUser appUser) {
    if (appUser.Email == null) {
      throw new InfrastructureException(
        $"Lỗi chuyển đổi dữ liệu: người dùng hệ thống không có email - Id: {appUser.Id}"
      );
    }

    return new User {
      Id = appUser.Id,
      FullName = appUser.FullName,
      Email = appUser.Email,
      PhoneNumber = appUser.PhoneNumber,
      Url = appUser.Url,
      IsActive = appUser.LockoutEnd == null
    };
  }

  private async Task<AppUser> FindOrThrowAsync(Guid id, CancellationToken ct) {
    var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    return user ?? throw new AppException($"Người dùng không tồn tại - Id: {id}.", 404);
  }
}