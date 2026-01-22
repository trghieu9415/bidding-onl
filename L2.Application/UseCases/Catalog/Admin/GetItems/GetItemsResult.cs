using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Catalog.Admin.GetItems;

public record GetItemsResult(List<CatalogItemDto> Items, Meta Meta);