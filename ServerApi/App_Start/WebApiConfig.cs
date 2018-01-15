using System.Net.Http.Formatting;
using System.Web.Http;
using ServerApi.Filters;

namespace ServerApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.Add(new JsonMediaTypeFormatter());

            config.Filters.Add(new ModelStateValidationActionFilter());
            config.Filters.Add(new ExceptionsActionFilter());

            config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
