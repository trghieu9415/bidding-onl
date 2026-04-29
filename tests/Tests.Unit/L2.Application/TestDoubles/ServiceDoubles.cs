using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.Ports.Cache;
using L2.Application.Ports.Search;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auctions.Commands.SearchItem;

namespace Tests.Unit.L2.Application.TestDoubles;

public sealed class StubSearchService : ISearchService {
  public (int total, List<AuctionSearchDto> items) SearchResult { get; set; } = (0, []);
  public AuctionSearchRequest? LastRequest { get; private set; }

  public Task<(int total, List<AuctionSearchDto> items)> SearchAsync(
    AuctionSearchRequest searchRequest,
    CancellationToken ct = default
  ) {
    LastRequest = searchRequest;
    return Task.FromResult(SearchResult);
  }
}

public sealed class StubBusinessCache : IBusinessCache {
  public List<AuctionSessionDto> CurrentSessionsResult { get; set; } = [];

  public Task<List<AuctionSessionDto>> GetCurrentSessionsAsync(CancellationToken ct) {
    return Task.FromResult(CurrentSessionsResult);
  }
}

public sealed class StubUserService : IUserService {
  public User? UserByIdResult { get; set; }
  public (int total, List<User> users) UsersResult { get; set; } = (0, []);
  public (int total, List<User> users) DeletedUsersResult { get; set; } = (0, []);
  public User? UpdatedUser { get; private set; }
  public Guid? LockedId { get; private set; }
  public Guid? UnlockedId { get; private set; }
  public UserFilter? LastFilter { get; private set; }
  public UserRole? LastRole { get; private set; }
  public Guid CreateResult { get; set; } = Guid.NewGuid();

  public Task<User?> GetByIdAsync(Guid id, UserRole? role, CancellationToken ct = default) {
    LastRole = role;
    return Task.FromResult(UserByIdResult);
  }

  public Task<(int total, List<User> users)> GetAsync(
    UserFilter? filter = null,
    UserRole? role = UserRole.Bidder,
    CancellationToken ct = default
  ) {
    LastFilter = filter;
    LastRole = role;
    return Task.FromResult(UsersResult);
  }

  public Task<(int total, List<User> users)> GetDeletedAsync(
    UserFilter? filter = null,
    UserRole? role = UserRole.Bidder,
    CancellationToken ct = default
  ) {
    LastFilter = filter;
    LastRole = role;
    return Task.FromResult(DeletedUsersResult);
  }

  public Task<Guid> CreateAsync(
    User user,
    string password,
    UserRole role,
    CancellationToken ct = default
  ) {
    LastRole = role;
    return Task.FromResult(CreateResult);
  }

  public Task UpdateAsync(User user, CancellationToken ct = default) {
    UpdatedUser = user;
    return Task.CompletedTask;
  }

  public Task DeleteAsync(Guid id, CancellationToken ct = default) => Task.CompletedTask;
  public Task RestoreAsync(Guid id, CancellationToken ct = default) => Task.CompletedTask;

  public Task LockAsync(Guid id, CancellationToken ct = default) {
    LockedId = id;
    return Task.CompletedTask;
  }

  public Task UnlockAsync(Guid id, CancellationToken ct = default) {
    UnlockedId = id;
    return Task.CompletedTask;
  }
}

public sealed class StubAuthService : IAuthService {
  public AuthTokens LoginResult { get; set; } = new(
    new TokenModel { Token = "access", ExpiredAt = DateTime.UtcNow.AddHours(1) },
    new TokenModel { Token = "refresh", ExpiredAt = DateTime.UtcNow.AddDays(1) }
  );
  public AuthTokens RegisterResult { get; set; } = new(
    new TokenModel { Token = "access", ExpiredAt = DateTime.UtcNow.AddHours(1) },
    new TokenModel { Token = "refresh", ExpiredAt = DateTime.UtcNow.AddDays(1) }
  );
  public AuthTokens RefreshResult { get; set; } = new(
    new TokenModel { Token = "access", ExpiredAt = DateTime.UtcNow.AddHours(1) },
    new TokenModel { Token = "refresh", ExpiredAt = DateTime.UtcNow.AddDays(1) }
  );
  public AuthTokens ChangePasswordResult { get; set; } = new(
    new TokenModel { Token = "access", ExpiredAt = DateTime.UtcNow.AddHours(1) },
    new TokenModel { Token = "refresh", ExpiredAt = DateTime.UtcNow.AddDays(1) }
  );
  public User? RegisteredUser { get; private set; }
  public string? RegisteredPassword { get; private set; }
  public (string email, string password, UserRole role)? LoginInput { get; private set; }
  public (Guid userId, string oldPassword, string newPassword)? ChangePasswordInput { get; private set; }
  public (string refreshToken, bool revokeAll)? LogoutInput { get; private set; }
  public string? RequestedPasswordEmail { get; private set; }
  public (string email, string token, string newPassword)? ResetPasswordInput { get; private set; }
  public string? RefreshedToken { get; private set; }

  public Task<AuthTokens> LoginAsync(string email, string password, UserRole role, CancellationToken ct = default) {
    LoginInput = (email, password, role);
    return Task.FromResult(LoginResult);
  }

  public Task<AuthTokens> RegisterAsync(User user, string password, CancellationToken ct = default) {
    RegisteredUser = user;
    RegisteredPassword = password;
    return Task.FromResult(RegisterResult);
  }

  public Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken ct = default) {
    RefreshedToken = refreshToken;
    return Task.FromResult(RefreshResult);
  }

  public Task LogoutAsync(string refreshToken, bool revokeAll, CancellationToken ct = default) {
    LogoutInput = (refreshToken, revokeAll);
    return Task.CompletedTask;
  }

  public Task<AuthTokens> ChangePasswordAsync(
    Guid userId,
    string oldPassword,
    string newPassword,
    CancellationToken ct = default
  ) {
    ChangePasswordInput = (userId, oldPassword, newPassword);
    return Task.FromResult(ChangePasswordResult);
  }

  public Task RequestPasswordAsync(string email, CancellationToken ct = default) {
    RequestedPasswordEmail = email;
    return Task.CompletedTask;
  }

  public Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default) {
    ResetPasswordInput = (email, token, newPassword);
    return Task.CompletedTask;
  }

  public Task<bool> ValidateSecurityStampAsync(Guid userId, string tokenSecurityStamp, CancellationToken ct = default) {
    return Task.FromResult(true);
  }
}
