using L1.Core.Domain.Transaction.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Gateway;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.CreatePayment;

public class CreatePaymentHandler(
  IRepository<Order> orderRepository,
  IRepository<Payment> paymentRepository,
  IGatewayFactory gatewayFactory,
  ICurrentUser currentUser
) : IRequestHandler<CreatePaymentCommand, CreatePaymentResult> {
  public async Task<CreatePaymentResult> Handle(CreatePaymentCommand request, CancellationToken ct) {
    var order = await orderRepository.GetByIdAsync(request.OrderId, ct)
                ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);

    if (order.BidderId != currentUser.Id) {
      throw new WorkflowException("Bạn không có quyền thanh toán đơn hàng này", 403);
    }

    var payment = Payment.Create(order.Id, order.Price, request.Method);
    await paymentRepository.CreateAsync(payment, ct);

    var gateway = gatewayFactory.CreatePaymentGateway(request.Method);
    var url = await gateway.CreatePaymentUrl(payment, order, ct);

    return new CreatePaymentResult(url);
  }
}
