using L2.Application.UseCases.Bidding.System.EndSession;
using L2.Application.UseCases.Bidding.System.StartSession;
using MassTransit;
using MediatR;

namespace L3.Worker.Consumers;

public class CommandConsumer(IMediator mediator) :
  IConsumer<EndSessionCommand>,
  IConsumer<StartSessionCommand> {
  public async Task Consume(ConsumeContext<EndSessionCommand> context) {
    await mediator.Send(context.Message, context.CancellationToken);
  }

  public async Task Consume(ConsumeContext<StartSessionCommand> context) {
    await mediator.Send(context.Message, context.CancellationToken);
  }
}
