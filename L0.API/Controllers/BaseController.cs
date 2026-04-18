using L2.Application.Ports.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase {
  private ICurrentUser? _currentUser;
  private IMediator? _mediator;
  protected ICurrentUser CurrentUser => _currentUser ??= HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
  protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
