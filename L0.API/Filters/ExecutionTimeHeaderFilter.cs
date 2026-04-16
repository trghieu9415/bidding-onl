using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace L0.API.Filters;

public class ExecutionTimeHeaderFilter : IAsyncActionFilter {
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
    var timer = Stopwatch.StartNew();
    await next();
    timer.Stop();
    var elapsedMilliseconds = timer.ElapsedMilliseconds;
    if (!context.HttpContext.Response.HasStarted) {
      context.HttpContext.Response.Headers.Append("X-Server-Execution-Time-ms", elapsedMilliseconds.ToString());
    }
  }
}
