using L1.Core.Domain.Catalog.Enums;
using L2.Application.Abstractions;

namespace L2.Application.UseCases.Catalog.Bidder.RegisterItem;

public record RegisterItemCommand(
  string Name,
  string Description,
  decimal StartingPrice,
  ItemCondition Condition,
  List<Guid> CategoryIds,
  string? MainImageUrl,
  List<string> SubImageUrls
) : ICommand<Guid>;