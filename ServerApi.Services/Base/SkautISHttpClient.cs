using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerApi.Services.Base
{
    internal class SkautIsHttpClient : HttpClient
    {
        public SkautIsHttpClient()
        {
            BaseAddress = new Uri("https://is.skaut.cz/JunakWebservice/");
        }

        public Task<HttpResponseMessage> PostSoapRequest(string serviceName, string operationName, XDocument bodyRequestTemplate,
            IDictionary<string, string> requestData)
        {
            var requestBody = PrepareSoapRequest(bodyRequestTemplate, requestData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{serviceName}.asmx");
            requestMessage.Headers.Add("SOAPAction", $"https://is.skaut.cz/{operationName}");
            requestMessage.Content = new StringContent(requestBody.ToString());
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

            return SendAsync(requestMessage);
        }

        private XDocument PrepareSoapRequest(XDocument bodyRequestTemplate, IDictionary<string, string> requestData)
        {
            foreach (var element in requestData)
            {
                bodyRequestTemplate.Descendants(XName.Get(element.Key, @"https://is.skaut.cz/")).First().SetValue(element.Value);
            }

            return bodyRequestTemplate;
        }
    }
}