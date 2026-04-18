using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using L1.Core.Domain.Bidding.Enums;

namespace L2.Application.Filters;

public class SessionFilter : BaseFilter {
  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Title { get; set; }

  public SessionStatus? Status { get; set; }

  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public DateTime? StartTime { get; set; }

  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public DateTime? EndTime { get; set; }
}
