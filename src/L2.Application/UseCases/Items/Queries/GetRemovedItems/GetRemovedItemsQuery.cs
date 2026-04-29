using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Items.Queries.GetRemovedItems;

public record GetRemovedItemsQuery(CatalogItemFilter Filter) : IRequest<GetRemovedItemsResult>;

public record GetRemovedItemsResult(List<CatalogItemDto> Items, Meta Meta);
