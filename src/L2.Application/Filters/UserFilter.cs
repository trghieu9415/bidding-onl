using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using FluentValidation;
using L2.Application.Models;

namespace L2.Application.Filters;

public class UserFilter : PaginationFilterBase {
  public Guid? Id { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? FullName { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Email { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? PhoneNumber { get; set; }

  public bool? IsActive { get; set; }

  public UserRole? Role { get; set; }
}

public sealed class UserFilterValidator : AbstractValidator<UserFilter> {
  public UserFilterValidator() {
    RuleFor(x => x.Page)
      .GreaterThan(0)
      .WithMessage("Số trang phải lớn hơn 0.");

    RuleFor(x => x.PerPage)
      .InclusiveBetween(1, 100)
      .WithMessage("Số lượng bản ghi mỗi trang phải từ 1 đến 100.");

    RuleFor(x => x.FullName)
      .MaximumLength(200)
      .WithMessage("Họ và tên không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.FullName));

    RuleFor(x => x.Email)
      .EmailAddress()
      .WithMessage("Email không hợp lệ.")
      .When(x => !string.IsNullOrWhiteSpace(x.Email));

    RuleFor(x => x.PhoneNumber)
      .Matches(@"^(0|\+84)[0-9]{9,10}$")
      .WithMessage("Số điện thoại không hợp lệ.")
      .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
  }
}
