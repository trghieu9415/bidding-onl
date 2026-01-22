using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Admin.AddSession;

public record AddSessionCommand(
  string Title,
  DateTime StartTime,
  DateTime EndTime,
  List<Guid> AuctionIds
) : ICommand<Guid>;