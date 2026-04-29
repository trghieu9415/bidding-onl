using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.CreatePayment;

public class CreatePaymentHandler(
  IRepository<Order> orderRepository,
  IRepository<Payment> paymentRepository,
  IGatewayFactory gatewayFactory
) : IRequestHandler<CreatePaymentCommand, CreatePaymentResult> {
  public async Task<CreatePaymentResult> Handle(CreatePaymentCommand request, CancellationToken ct) {
    var order =
      await orderRepository.GetByIdAsync(request.OrderId, ct)
      ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);

    if (order.BidderId != request.UserId) {
      throw new WorkflowException("Bạn không có quyền thanh toán đơn hàng này", 403);
    }

    var payment = await paymentRepository.GetFirstAsync(p => p.OrderId == order.Id, ct);

    if (payment is { Status: PaymentStatus.Pending }) {
      return new CreatePaymentResult(payment.PaymentUrl!);
    }

    var newPayment = Payment.Create(order.Id, order.Price, request.Method);
    var gateway = gatewayFactory.CreatePaymentGateway(request.Method);
    var url = await gateway.CreatePaymentUrl(newPayment, order, ct);
    newPayment.SetPaymentUrl(url);

    await paymentRepository.CreateAsync(newPayment, ct);
    return new CreatePaymentResult(url);
  }
}
