using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;

// Bro điều chỉnh namespace entity cho đúng nhé

namespace L2.Application.Filters;

public class CatalogItemFilter : BaseFilter {
  public Guid? OwnerId { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Name { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Description { get; set; }

  public ItemStatus? Status { get; set; }
  public ItemCondition? Condition { get; set; }

  [CompareTo(nameof(CatalogItem.StartingPrice))]
  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public decimal? MinStartingPrice { get; set; }

  [CompareTo(nameof(CatalogItem.StartingPrice))]
  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public decimal? MaxStartingPrice { get; set; }

  public Guid? CategoryIds { get; set; }
}
