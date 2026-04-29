using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Items.Queries.GetRegisteredItems;

public record GetRegisteredItemsQuery(Guid UserId, CatalogItemFilter Filter) : IRequest<GetRegisteredItemsResult>;

public record GetRegisteredItemsResult(List<CatalogItemDto> Items, Meta Meta);
