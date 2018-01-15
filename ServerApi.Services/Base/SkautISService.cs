using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerApi.Services.Base
{
    public abstract class SkautIsService
    {
        internal SkautIsHttpClient Client { get; }

        protected SkautIsService()
        {
            Client = new SkautIsHttpClient();
        }

        protected async Task<XDocument> GetResponseXmlAsync(HttpResponseMessage response)
        {
            XDocument responseXml;

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                responseXml = XDocument.Load(responseStream);
            }

            return responseXml;
        }
    }
}
