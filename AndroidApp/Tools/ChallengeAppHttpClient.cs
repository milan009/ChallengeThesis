using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OdborkyApp.Tools
{
    internal class ChallengeAppHttpClient
    {
        private const string BaseUrl = "https://challengethesis.azurewebsites.net/api/";
        private DateTime? _lastSyncTime;

        public ChallengeAppHttpClient(DateTime? lastSyncTime = null)
        {
            _lastSyncTime = lastSyncTime;
        }

        public IEnumerable<T> FetchCollection<T>(string endpointPath, string xmlElementName)
        {
            XDocument responseXml;
            List<T> fetchCollection = new List<T>();
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            var url = BaseUrl + endpointPath;
            if (_lastSyncTime != null)
                url += $"?lastUpdate={_lastSyncTime.Value.Ticks}";

            var request = new HttpWebRequest(new System.Uri(url))
            {
                Method = "GET",
                ContentType = "text/xml",
            };

            request.Headers.Add("DeviceGuid", AppState.State.Instance.DeviceGuid.ToString());

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                responseXml = XDocument.Load(responseStream);
            }

            foreach (var xmlElement in responseXml.Descendants(xmlElementName))
            {
                using (var reader = xmlElement.CreateReader())
                {
                    fetchCollection.Add((T)serializer.Deserialize(reader));
                }
            }

            return fetchCollection;
        }

        public void PutCollectionByElements<T>(string endpointPath, IEnumerable<T> collection, Func<T, string> idExtractor)
        {
            foreach (var element in collection)
            {
                var url = BaseUrl + endpointPath + "/" + idExtractor(element);

                var request = new HttpWebRequest(new System.Uri(url))
                {
                    Method = "PUT",
                    ContentType = "Application/json",
                };

                request.Headers.Add("DeviceGuid", AppState.State.Instance.DeviceGuid.ToString());

                using (var stream = request.GetRequestStream())
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(element));
                }

                    
                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse) response).StatusCode != HttpStatusCode.Created)
                    {
                        throw new WebException("PUT request failed");
                    }
                }
            }
        }
    }
}