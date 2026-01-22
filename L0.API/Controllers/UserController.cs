using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[ApiController]
[Route("api/user/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public abstract class UserController : ControllerBase;