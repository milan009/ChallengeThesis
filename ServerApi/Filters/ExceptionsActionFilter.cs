using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace ServerApi.Filters
{
    public class ExceptionsActionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response =
                actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    actionExecutedContext.Exception);
        }
    }
}