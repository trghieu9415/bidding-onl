using FluentValidation;

namespace L2.Application.UseCases.Catalog.Admin.AddCategory;

public class AddCategoryValidator : AbstractValidator<AddCategoryCommand> {
  public AddCategoryValidator() {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Tên danh mục không được để trống");
  }
}