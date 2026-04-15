using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.GetRegisteredItems;

public record GetRegisteredItemsQuery(SieveModel SieveModel) : IQuery<GetRegisteredItemsResult>;

public record GetRegisteredItemsResult(List<CatalogItemDto> Items, Meta Meta);
