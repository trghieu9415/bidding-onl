using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.GetItem;

public class GetItemHandler(IReadRepository<CatalogItem> readRepository, IMapper mapper)
  : IRequestHandler<GetItemQuery, GetItemResult> {
  public async Task<GetItemResult> Handle(GetItemQuery request, CancellationToken ct) {
    var item = await readRepository.GetByIdAsync(request.Id, ct)
               ?? throw new AppException("Sản phẩm không tồn tại", 404);

    var dto = mapper.Map<CatalogItemDto>(item);
    return new GetItemResult(dto);
  }
}
