using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[ApiController]
[Route("api/external/[controller]")]
[ApiExplorerSettings(GroupName = "v3")]
public abstract class ExternalController;
