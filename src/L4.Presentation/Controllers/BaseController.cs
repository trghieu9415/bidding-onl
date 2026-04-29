using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace L4.Presentation.Controllers;

[ApiController]
[EnableRateLimiting("PublicApiPolicy")]
public abstract class BaseController : ControllerBase {
  private IMediator? _mediator;
  protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
