using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.AddCategory;

public record AddCategoryCommand(string Name, Guid? ParentId) : IRequest<Guid>, ITransactional;

public sealed class AddCategoryValidator : AbstractValidator<AddCategoryCommand> {
  public AddCategoryValidator() {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Tên danh mục không được để trống.")
      .MaximumLength(200)
      .WithMessage("Tên danh mục không được vượt quá 200 ký tự.");
  }
}
