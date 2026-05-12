using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using FluentValidation;
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

public sealed class SessionFilterValidator : BaseFilterValidator<SessionFilter> {
  public SessionFilterValidator() {
    RuleFor(x => x.Title)
      .MaximumLength(200)
      .WithMessage("Tiêu đề phiên đấu giá không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.Title));

    RuleFor(x => x.EndTime)
      .GreaterThanOrEqualTo(x => x.StartTime!.Value)
      .WithMessage("Thời gian bắt đầu không được lớn hơn thời gian kết thúc.")
      .When(x => x.StartTime.HasValue && x.EndTime.HasValue);
  }
}
