using System.Diagnostics.CodeAnalysis;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;

namespace Tests.Common.Builders;

[ExcludeFromCodeCoverage]
public class CatalogItemBuilder {
  private Guid[]? _categoryIds;
  private ItemCondition? _condition;
  private string _description = "Gaming laptop";
  private string? _mainImageUrl;
  private string _name = "Laptop";
  private Guid _ownerId = Guid.NewGuid();
  private decimal? _startingPrice;
  private string[]? _subImageUrls;

  public CatalogItemBuilder WithOwnerId(Guid ownerId) {
    _ownerId = ownerId;
    return this;
  }

  public CatalogItemBuilder WithName(string name) {
    _name = name;
    return this;
  }

  public CatalogItemBuilder WithDescription(string description) {
    _description = description;
    return this;
  }

  public CatalogItemBuilder WithCondition(ItemCondition condition) {
    _condition = condition;
    return this;
  }

  public CatalogItemBuilder WithStartingPrice(decimal startingPrice) {
    _startingPrice = startingPrice;
    return this;
  }

  public CatalogItemBuilder WithCategories(Guid[] categoryIds) {
    _categoryIds = categoryIds;
    return this;
  }

  public CatalogItemBuilder WithImages(string mainImageUrl, string[] subImageUrls) {
    _mainImageUrl = mainImageUrl;
    _subImageUrls = subImageUrls;
    return this;
  }

  public CatalogItem Build() {
    var item = CatalogItem.Create(_ownerId, _name, _description);

    if (_condition.HasValue) {
      item.SetCondition(_condition.Value);
    }

    if (_startingPrice.HasValue) {
      item.SetStartingPrice(_startingPrice.Value);
    }

    if (_categoryIds != null) {
      item.SyncCategories(_categoryIds);
    }

    if (_mainImageUrl != null || _subImageUrls != null) {
      item.SetImageUrls(_mainImageUrl, _subImageUrls ?? []);
    }

    return item;
  }
}
