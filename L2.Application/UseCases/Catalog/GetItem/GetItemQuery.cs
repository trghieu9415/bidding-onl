using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Catalog.GetItem;

public record GetItemQuery(Guid Id) : IRequest<GetItemResult>;

public record GetItemResult(CatalogItemDto Item);
