using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Items.Queries.GetRegisteredItems;

public record GetRegisteredItemsQuery(Guid UserId, SieveModel SieveModel) : IRequest<GetRegisteredItemsResult>;

public record GetRegisteredItemsResult(List<CatalogItemDto> Items, Meta Meta);
