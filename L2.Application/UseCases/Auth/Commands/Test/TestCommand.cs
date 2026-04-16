using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Test;

public record TestCommand(string Id, int Value) : IRequest<bool>, ILockable {
  public string LockKey => $"locks:test:{Id}";
  public TimeSpan WaitTime => TimeSpan.FromSeconds(5);
}
