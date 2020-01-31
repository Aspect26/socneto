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
            if (context.Exception is ServiceUnavailableException)
            {
                context.ExceptionHandled = true;
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }

            base.OnActionExecuted(context);
        }
    }
}