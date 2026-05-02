using AutoMapper;
using AutoMapper.QueryableExtensions;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Repositories.Read;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Repositories.Read;

public class PaymentReadRepository(
  AppDbContext dbContext,
  IMapper mapper
) : EfReadRepository<Payment, PaymentDto>(dbContext, mapper), IPaymentReadRepository {
  private readonly IMapper _mapper = mapper;

  public async Task<PaymentDto?> GetByTransactionIdAsync(string transactionId, CancellationToken ct = default) {
    return await DbSet
      .AsNoTracking()
      .Where(x => x.TransactionId == transactionId)
      .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
      .FirstOrDefaultAsync(ct);
  }

  public async Task<PaymentDto?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default) {
    return await DbSet
      .AsNoTracking()
      .Where(x => x.OrderId == orderId)
      .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
      .FirstOrDefaultAsync(ct);
  }
}
