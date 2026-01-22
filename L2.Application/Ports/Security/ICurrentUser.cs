using L2.Application.Models;

namespace L2.Application.Ports.Security;

public interface ICurrentUser {
  User User { get; init; }
}