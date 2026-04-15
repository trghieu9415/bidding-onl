using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.UpdateSession;

public record UpdateSessionCommand(
  Guid Id,
  string Title,
  DateTime StartTime,
  DateTime EndTime
) : IRequest<Unit>, ITransactional;
