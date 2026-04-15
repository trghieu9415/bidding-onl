using FluentValidation;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.UpdateRegisteredItem;

public record UpdateRegisteredItemCommand(
  Guid Id,
  string? Name,
  string? Description,
  decimal? StartingPrice,
  ItemCondition? Condition,
  List<Guid>? CategoryIds,
  string? MainImageUrl,
  List<string>? SubImageUrls
) : IRequest<Unit>, ITransactional;

public class UpdateRegisteredItemValidator : AbstractValidator<UpdateRegisteredItemCommand> {
  public UpdateRegisteredItemValidator() {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.StartingPrice)
      .GreaterThan(0)
      .When(x => x.StartingPrice.HasValue);
  }
}
