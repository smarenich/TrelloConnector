using Newtonsoft.Json;
using PX.Commerce.Core;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrelloConnector
{
	[Description("Card")]
	public class CardData : BCAPIEntity
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("desc")]
		[Description("Description")]
		public string Description { get; set; }

		[JsonProperty("name")]
		[Description("Name")]
		public String Name { get; set; }

		[JsonProperty("dateLastActivity")]
		public DateTime LastActivity { get; set; }
	}

    public class TrelloRestClient : RestClient
    {
        protected ISerializer _serializer;
        protected IDeserializer _deserializer;

        public TrelloRestClient(String url, IDeserializer deserializer, ISerializer serializer)
        {
            _serializer = serializer;
            _deserializer = deserializer;
            AddHandler("application/json", deserializer);
            AddHandler("text/json", deserializer);

            try
            {
                BaseUrl = new Uri(url);
            }
            catch (UriFormatException e)
            {
                throw new UriFormatException("Invalid URL: The format of the URL could not be determined.", e);
            }
        }

        public RestRequest MakeRequest(string url, Dictionary<string, string> urlSegments = null)
        {
            var request = new RestRequest(url) { JsonSerializer = _serializer, RequestFormat = DataFormat.Json };

            if (urlSegments != null)
            {
                foreach (var urlSegment in urlSegments)
                {
                    request.AddUrlSegment(urlSegment.Key, urlSegment.Value);
                }
            }
            return request;
        }

		public T Get<T>(string url)
			where T : class, new()
		{
            RestRequest request = MakeRequest(url);

            request.Method = Method.GET;
			var response = Execute<T>(request);

			if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
			{
				T result = response.Data;

				if (result != null && result is BCAPIEntity) (result as BCAPIEntity).JSON = response.Content;

				return result;
			}

			throw new Exception(response.Content);
		}
	}

	    public class RestJsonSerializer : ISerializer, IDeserializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public RestJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json";           
            _serializer = serializer;
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        public string DateFormat { get; set; }
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string ContentType { get; set; }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            String content = response.Content;
			JsonSerializerSettings settings = new JsonSerializerSettings { };

			T result = JsonConvert.DeserializeObject<T>(content, settings);

			return result;
		}
    }
}
