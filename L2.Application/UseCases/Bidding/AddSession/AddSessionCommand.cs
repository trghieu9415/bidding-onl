using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.AddSession;

public record AddSessionCommand(
  string Title,
  DateTime StartTime,
  DateTime EndTime,
  List<Guid> AuctionIds
) : IRequest<Guid>, ITransactional;
