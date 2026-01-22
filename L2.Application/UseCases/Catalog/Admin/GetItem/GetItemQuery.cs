using L2.Application.Abstractions;

namespace L2.Application.UseCases.Catalog.Admin.GetItem;

public record GetItemQuery(Guid Id) : IQuery<GetItemResult>;