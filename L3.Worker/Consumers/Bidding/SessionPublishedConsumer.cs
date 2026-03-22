using L1.Core.Domain.Bidding.Events;
using L2.Application.UseCases.Bidding.System.EndSession;
using L2.Application.UseCases.Bidding.System.StartSession;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class SessionPublishedConsumer(
  IMessageScheduler scheduler
) : IConsumer<SessionPublishedEvent> {
  public async Task Consume(ConsumeContext<SessionPublishedEvent> context) {
    var msg = context.Message;
    await scheduler.SchedulePublish(msg.StartTime, new StartSessionCommand(msg.Id));
    await scheduler.SchedulePublish(msg.EndTime, new EndSessionCommand(msg.Id));
  }
}
