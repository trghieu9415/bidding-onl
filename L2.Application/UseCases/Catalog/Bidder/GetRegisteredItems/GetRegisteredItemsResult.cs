using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Catalog.Bidder.GetRegisteredItems;

public record GetRegisteredItemsResult(List<CatalogItemDto> Items, Meta Meta);