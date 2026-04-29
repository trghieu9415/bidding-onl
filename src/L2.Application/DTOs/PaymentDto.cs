using L1.Core.Domain.Transaction.Enums;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record PaymentDto : IdDto {
  public Guid OrderId { get; init; }
  public decimal Amount { get; init; }
  public string? PaymentUrl { get; init; }
  public string? TransactionId { get; init; }
  public PaymentMethod Method { get; init; }
  public PaymentStatus Status { get; init; }
}
