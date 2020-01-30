using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Socneto.Domain.Services;

namespace Socneto.Api.Controllers
{
    public class SocnetoController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var responseCode = Response.StatusCode;
            var exception = context.Exception;

            if (exception is ServiceUnavailableException)
            {
                responseCode = StatusCodes.Status503ServiceUnavailable;
                context.ExceptionHandled = true;
            }

            Response.StatusCode = responseCode;

            base.OnActionExecuted(context);
        }
    }
}