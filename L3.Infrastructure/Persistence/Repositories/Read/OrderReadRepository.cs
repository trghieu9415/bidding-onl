using AutoMapper;
using AutoMapper.QueryableExtensions;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Repositories.Read;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace L3.Infrastructure.Persistence.Repositories.Read;

public class OrderReadRepository(
  AppDbContext dbContext,
  IMapper mapper,
  ISieveProcessor sieveProcessor
) : EfReadRepository<Order, OrderDto>(dbContext, mapper, sieveProcessor), IOrderReadRepository {
  private readonly IMapper _mapper = mapper;

  public virtual async Task<OrderDto?> GetByAuctionIdAsync(Guid auctionId, CancellationToken ct = default) {
    return await DbSet
      .AsNoTracking()
      .Where(x => x.AuctionId == auctionId && !x.IsDeleted)
      .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
      .FirstOrDefaultAsync(ct);
  }

  public async Task<(OrderDto? order, List<PaymentDto> payments)> GetOrderPaymentByIdAsync(Guid orderId,
    CancellationToken ct = default) {
    var orderDto = await DbSet
      .AsNoTracking()
      .Where(x => x.Id == orderId && !x.IsDeleted)
      .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
      .FirstOrDefaultAsync(ct);

    if (orderDto == null) {
      return (null, []);
    }

    var paymentDtos = await dbContext.Set<Payment>()
      .AsNoTracking()
      .Where(x => x.OrderId == orderId && !x.IsDeleted)
      .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
      .ToListAsync(ct);

    return (orderDto, paymentDtos);
  }
}
