using FluentValidation;

namespace L2.Application.UseCases.Catalog.Bidder.UpdateRegisteredItem;

public class UpdateRegisteredItemValidator : AbstractValidator<UpdateRegisteredItemCommand> {
  public UpdateRegisteredItemValidator() {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.StartingPrice)
      .GreaterThan(0)
      .When(x => x.StartingPrice.HasValue);
  }
}