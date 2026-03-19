using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.GetItem;

public class GetItemHandler(IReadRepository<CatalogItem, CatalogItemDto> readRepository)
  : IRequestHandler<GetItemQuery, GetItemResult> {
  public async Task<GetItemResult> Handle(GetItemQuery request, CancellationToken ct) {
    var item = await readRepository.GetByIdAsync(request.Id, ct)
               ?? throw new WorkflowException("Sản phẩm không tồn tại", 404);
    return new GetItemResult(item);
  }
}
