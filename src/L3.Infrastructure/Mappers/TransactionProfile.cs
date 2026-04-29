using AutoMapper;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;

namespace L3.Infrastructure.Mappers;

public class TransactionProfile : Profile {
  public TransactionProfile() {
    CreateMap<Order, OrderDto>();
    CreateMap<Payment, PaymentDto>();
  }
}
