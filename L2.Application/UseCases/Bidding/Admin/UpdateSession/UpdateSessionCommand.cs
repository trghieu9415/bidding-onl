using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.UpdateSession;

public record UpdateSessionCommand(
  Guid Id,
  string Title,
  DateTime StartTime,
  DateTime EndTime
) : ICommand<Unit>;