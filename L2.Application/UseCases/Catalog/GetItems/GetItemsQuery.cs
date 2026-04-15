using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.GetItems;

public record GetItemsQuery(SieveModel SieveModel) : IQuery<GetItemsResult>;

public record GetItemsResult(List<CatalogItemDto> Items, Meta Meta);
