using FluentValidation;

namespace L2.Application.UseCases.Catalog.Admin.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand> {
  public UpdateCategoryValidator() {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
  }
}