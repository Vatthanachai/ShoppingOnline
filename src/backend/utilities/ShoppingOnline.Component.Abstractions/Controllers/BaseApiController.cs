using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShoppingOnline.Component.Abstractions.ServiceResponses;

namespace ShoppingOnline.Component.Abstractions.Controllers;

public class BaseApiController : ControllerBase
{
    protected IActionResult ReturnResponseWithHttpStatus(ServiceResponse response)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));

        var responseType = response.GetType();
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Service200Response<>))
        {
            return Ok(response);
        }

        return response switch
        {
            Service200Response item => Ok(item),
            Service200Response<object> item => Ok(item),
            Service201Response created => Created(),
            Service204Response noContent => NoContent(),
            Service400Response badRequest => BadRequest(badRequest.Message),
            Service401Response unAuthorize => Unauthorized(unAuthorize.Message),
            Service403Response forbid => Forbid(forbid.Message),
            Service404Response notFound => NotFound(notFound.Message),
            Service409Response conflict => Conflict(conflict.Message),
            Service500Response internalServerError => StatusCode(StatusCodes.Status500InternalServerError,
                internalServerError.Exception),
            _ => throw new ArgumentOutOfRangeException(nameof(response))
        };
    }
}