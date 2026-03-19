using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;

namespace L2.Application.Repositories.Read;

public interface IPaymentReadRepository : IReadRepository<Payment, PaymentDto> {
  Task<PaymentDto?> GetByTransactionIdAsync(string transactionId, CancellationToken ct = default);
  Task<PaymentDto?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
