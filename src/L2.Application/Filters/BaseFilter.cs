using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using FluentValidation;

namespace L2.Application.Filters;

public abstract class BaseFilter : PaginationFilterBase {
  public Guid? Id { get; set; }

  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public DateTime? MinCreatedAt { get; set; }

  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public DateTime? MaxCreatedAt { get; set; }
}

public abstract class BaseFilterValidator<T> : AbstractValidator<T> where T : BaseFilter {
  protected BaseFilterValidator() {
    RuleFor(x => x.Page)
      .GreaterThan(0)
      .WithMessage("Số trang phải lớn hơn 0.");

    RuleFor(x => x.PerPage)
      .InclusiveBetween(1, 100)
      .WithMessage("Số lượng bản ghi mỗi trang phải từ 1 đến 100.");

    RuleFor(x => x.MaxCreatedAt)
      .GreaterThanOrEqualTo(x => x.MinCreatedAt!.Value)
      .WithMessage("Ngày tạo bắt đầu không được lớn hơn ngày tạo kết thúc.")
      .When(x => x.MinCreatedAt.HasValue && x.MaxCreatedAt.HasValue);
  }
}
