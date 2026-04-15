using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.System.StartSession;

public record StartSessionCommand(Guid Id) : ICommand<Unit> {
  public StartSessionCommand() : this(Guid.Empty) {}
}
