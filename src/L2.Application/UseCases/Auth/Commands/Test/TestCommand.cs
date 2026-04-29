using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Test;

public record TestCommand(Guid UserId, TestRequest Data) : IRequest<bool>, ILockable {
  public string LockKey => $"locks:test:{Data.Id}";
  public TimeSpan WaitTime => TimeSpan.FromSeconds(5);
}

public record TestRequest(string Id, int Value);
