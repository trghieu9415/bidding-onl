using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.System.EndSession;

public record EndSessionCommand(Guid Id) : ICommand<Unit> {
  public EndSessionCommand() : this(Guid.Empty) {}
}
