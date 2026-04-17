using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace L0.API.Controllers;

[Route("api/user/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
[EnableRateLimiting("PublicApiPolicy")]
public abstract class UserController : BaseController;
