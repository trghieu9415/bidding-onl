using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.GetItems;

public record GetItemsQuery(SieveModel SieveModel) : IRequest<GetItemsResult>;

public record GetItemsResult(List<CatalogItemDto> Items, Meta Meta);
