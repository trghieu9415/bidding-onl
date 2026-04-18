using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Items.Queries.GetItems;

public record GetItemsQuery(CatalogItemFilter Filter) : IRequest<GetItemsResult>;

public record GetItemsResult(List<CatalogItemDto> Items, Meta Meta);
