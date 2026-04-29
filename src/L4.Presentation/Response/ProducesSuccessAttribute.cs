using Microsoft.AspNetCore.Mvc;

namespace L4.Presentation.Response;

public class ProducesSuccessAttribute<T>(int status = 200)
  : ProducesResponseTypeAttribute(typeof(ResponseData<T>), status);
