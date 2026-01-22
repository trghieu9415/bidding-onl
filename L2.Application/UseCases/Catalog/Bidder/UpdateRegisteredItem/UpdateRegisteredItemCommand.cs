using L1.Core.Domain.Catalog.Enums;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.Bidder.UpdateRegisteredItem;

public record UpdateRegisteredItemCommand(
  Guid Id,
  string? Name,
  string? Description,
  decimal? StartingPrice,
  ItemCondition? Condition,
  List<Guid>? CategoryIds,
  string? MainImageUrl,
  List<string>? SubImageUrls
) : ICommand<Unit>;