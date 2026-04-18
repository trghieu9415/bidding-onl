using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
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
