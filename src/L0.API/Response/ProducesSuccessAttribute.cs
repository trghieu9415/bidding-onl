using Microsoft.AspNetCore.Mvc;

namespace L0.API.Response;

public class ProducesSuccessAttribute<T>(int status = 200)
  : ProducesResponseTypeAttribute(typeof(ResponseData<T>), status);
