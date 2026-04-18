using AutoFilterer.Attributes;
using AutoFilterer.Enums;

namespace L2.Application.Filters;

public class CategoryFilter : BaseFilter {
  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Name { get; set; }

  public Guid? ParentId { get; set; }
}
