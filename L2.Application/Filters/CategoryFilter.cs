using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using FluentValidation;

namespace L2.Application.Filters;

public class CategoryFilter : BaseFilter {
  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Name { get; set; }

  public Guid? ParentId { get; set; }
}

public sealed class CategoryFilterValidator : BaseFilterValidator<CategoryFilter> {
  public CategoryFilterValidator() {
    RuleFor(x => x.Name)
      .MaximumLength(200)
      .WithMessage("Tên danh mục không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.Name));
  }
}
