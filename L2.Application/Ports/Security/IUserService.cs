using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.Ports.Identity;

public interface IUserService {
  Task<User?> GetByIdAsync(Guid id,
    UserRole role = UserRole.Bidder,
    CancellationToken ct = default
  );

  Task<(int total, List<User> users)> GetAsync(
    SieveModel? sieveModel = null,
    UserRole role = UserRole.Bidder,
    CancellationToken ct = default
  );

  Task<(int total, List<User> users)> GetDeletedAsync(
    SieveModel? sieveModel = null,
    UserRole role = UserRole.Bidder,
    CancellationToken ct = default
  );

  Task<Guid> CreateAsync(
    User user,
    string password,
    UserRole role,
    CancellationToken ct = default
  );

  Task UpdateAsync(User user, CancellationToken ct = default);
  Task DeleteAsync(Guid id, CancellationToken ct = default);
  Task RestoreAsync(Guid id, CancellationToken ct = default);

  Task LockAsync(Guid id, CancellationToken ct = default);
  Task UnlockAsync(Guid id, CancellationToken ct = default);
}
