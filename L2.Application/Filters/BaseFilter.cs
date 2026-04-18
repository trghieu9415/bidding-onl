using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;

namespace L2.Application.Filters;

public abstract class BaseFilter : PaginationFilterBase {
  public Guid? Id { get; set; }

  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public DateTime? MinCreatedAt { get; set; }

  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public DateTime? MaxCreatedAt { get; set; }
}
