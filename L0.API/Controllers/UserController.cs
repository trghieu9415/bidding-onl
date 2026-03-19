using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[Route("api/user/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public abstract class UserController : BaseController;
