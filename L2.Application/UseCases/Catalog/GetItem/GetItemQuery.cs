using L2.Application.Abstractions;
using L2.Application.DTOs;

namespace L2.Application.UseCases.Catalog.GetItem;

public record GetItemQuery(Guid Id) : IQuery<GetItemResult>;

public record GetItemResult(CatalogItemDto Item);
