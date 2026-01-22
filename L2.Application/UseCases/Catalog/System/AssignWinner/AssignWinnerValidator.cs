using FluentValidation;

namespace L2.Application.UseCases.Catalog.System.AssignWinner;

public class AssignWinnerValidator : AbstractValidator<AssignWinnerCommand> {
  public AssignWinnerValidator() {
    RuleFor(x => x.CatalogItemId)
      .NotEmpty().WithMessage("Id sản phẩm không được để trống");
  }
}