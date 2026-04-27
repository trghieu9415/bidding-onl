using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, UpdateCategoryRequest Data) : IRequest<bool>, ITransactional;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest> {
  public UpdateCategoryValidator() {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Tên danh mục không được để trống.")
      .MaximumLength(200)
      .WithMessage("Tên danh mục không được vượt quá 200 ký tự.");
  }
}

public record UpdateCategoryRequest(string Name, Guid? ParentId);
