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
            // NOT Forbid(forbid.Message) - ControllerBase.Forbid(string) treats its argument as
            // an authentication scheme name to challenge, not a response body. Passing an
            // arbitrary message there breaks routing/auth resolution instead of returning 403.
            Service403Response forbid => StatusCode(StatusCodes.Status403Forbidden, forbid.Message),
            Service404Response notFound => NotFound(notFound.Message),
            Service409Response conflict => Conflict(conflict.Message),
            Service500Response internalServerError => StatusCode(StatusCodes.Status500InternalServerError,
                internalServerError.Exception),
            _ => throw new ArgumentOutOfRangeException(nameof(response))
        };
    }
}