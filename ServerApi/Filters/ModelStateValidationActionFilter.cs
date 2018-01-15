using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ServerApi.Filters
{
    public class ModelStateValidationActionFilter : ActionFilterAttribute
    {
        // https://stackoverflow.com/questions/41872079/validate-modelstate-isvalid-globally-for-all-controllers
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;

            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, modelState);
            }
        }
    }
}