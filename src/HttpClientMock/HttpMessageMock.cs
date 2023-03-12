using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HttpClientMock
{
    public struct HttpMessageMock
    {
        private static Random _random = new Random();
        private static HttpStatusCode[] FailHttpStatusCode = new[]
        {
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest,
            HttpStatusCode.BadGateway,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.NetworkAuthenticationRequired,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.FailedDependency,
            HttpStatusCode.Forbidden
        };

        public HttpMethod Method { get; private set; }
        public string Path { get; private set; }
        public ContentFormat ResponseContentFormat { get; private set; }
        public object? ResponseContent { get; private set; }

        private HttpStatusCode ResponseStatusCode;

        internal static string[] MockKeyComposition(HttpMessageMock mock) 
            => new[]
            {
                mock.Method.ToString(),
                new Uri(Helpers.DEFAULT_BASE_URL).Combine(mock.Path).PathAndQuery
            };
        internal static string[] MockKeyComposition(HttpRequestMessage request)
            => new[]
            {
                request.Method.ToString(),
                new Uri(Helpers.DEFAULT_BASE_URL).Combine(request.RequestUri.PathAndQuery).PathAndQuery ?? string.Empty
            };

        internal static string GetMockKey(HttpMessageMock mock) 
            => string.Join("_", MockKeyComposition(mock));

        internal static string GetMockKey(HttpRequestMessage request)
            => string.Join("_", MockKeyComposition(request));

        internal HttpMessageMock(HttpRequestMessage request, HttpStatusCode responseCode)
            : this(request, responseCode, null) { }
        
        internal HttpMessageMock(HttpRequestMessage request, HttpStatusCode responseCode, object? responseContent)
            : this(request.Method, request.RequestUri?.PathAndQuery ?? string.Empty, ContentFormat.Json, responseContent, responseCode) { }

        internal HttpMessageMock(HttpMethod method, string path, object? responseContent, HttpStatusCode responseStatusCode)
            : this(method, path, ContentFormat.Json, responseContent, responseStatusCode) { }

        internal HttpMessageMock(HttpMethod method, string path, ContentFormat responseContentFormat, object? responseContent, HttpStatusCode responseStatusCode)
        {
            Method = method;
            Path = new Uri(Helpers.DEFAULT_BASE_URL).Combine(path).PathAndQuery;
            ResponseContentFormat = responseContentFormat;
            ResponseContent = responseContent;
            ResponseStatusCode = responseStatusCode;
        }

        internal static HttpResponseMessage Success(HttpRequestMessage request)
            => new HttpMessageMock(request, HttpStatusCode.OK)
                .AsHttpResponseMessage();

        internal static HttpResponseMessage Fail(HttpRequestMessage request)
        {
            var selectedIndex = _random.Next(FailHttpStatusCode.Length);

            return new HttpMessageMock(request, FailHttpStatusCode[selectedIndex])
                .AsHttpResponseMessage();
        }

        public static HttpMessageMock Get(string path, object response)
            => new HttpMessageMock(HttpMethod.Get, path, ContentFormat.Json, response, HttpStatusCode.OK);
        public static HttpMessageMock Post(string path, object response)
            => new HttpMessageMock(HttpMethod.Post, path, ContentFormat.Json, response, HttpStatusCode.Created);

        internal HttpResponseMessage AsHttpResponseMessage()
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = ResponseStatusCode,
                Content = CreateReponse()
            };

            return response;
        }

        internal static string GetKey(HttpRequestMessage request)
            => request.Method.ToString();

        private HttpContent CreateReponse()
        {
            string responseContent;

            switch (ResponseContentFormat)
            {
                case ContentFormat.Json:
                    responseContent = JsonConvert.SerializeObject(ResponseContent ?? new { });
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new StringContent(
                responseContent,
                Encoding.UTF8,
                "application/json");
        }
    }
}
