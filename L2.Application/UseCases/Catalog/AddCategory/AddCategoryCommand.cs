using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.AddCategory;

public record AddCategoryCommand(string Name, Guid? ParentId) : IRequest<Guid>, ITransactional;

public class AddCategoryValidator : AbstractValidator<AddCategoryCommand> {
  public AddCategoryValidator() {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Tên danh mục không được để trống");
  }
}
