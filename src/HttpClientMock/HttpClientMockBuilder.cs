using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace HttpClientMock
{
    public class HttpClientMockBuilder
    {
        private Uri BaseUrl = new Uri(Helpers.DEFAULT_BASE_URL);
        private Behavior DefaultBehavior = Behavior.FakeAsSuccess;
        private IDictionary<string, HttpMessageMock> MockDictionary = new Dictionary<string, HttpMessageMock>();

        public HttpClientMockBuilder UsingBaseUrl(string value)
        {
            BaseUrl = new Uri(value);
            return this;
        }
        public HttpClientMockBuilder WithDefaultBehavior(Behavior value)
        {
            DefaultBehavior = value;
            return this;
        }

        public HttpClientMockBuilder Mocking(string path)
            => Mocking(HttpMethod.Get, path);

        public HttpClientMockBuilder Mocking(string path, object? response)
            => Mocking(HttpMethod.Get, path, response);

        public HttpClientMockBuilder Mocking(HttpMethod method, string path)
            => Mocking(method, path, HttpStatusCode.OK);

        public HttpClientMockBuilder Mocking(HttpMethod method, string path, HttpStatusCode responseStatus)
            => Mocking(method, path, null, responseStatus);

        public HttpClientMockBuilder Mocking(HttpMethod method, string path, object? responseContent)
            => Mocking(method, path, responseContent, HttpStatusCode.OK);

        public HttpClientMockBuilder Mocking(HttpMethod method, string path, object? responseContent, HttpStatusCode responseStatus)
            => Mocking(new HttpMessageMock(method, path, responseContent, responseStatus));

        public HttpClientMockBuilder Mocking(HttpMessageMock value)
        {
            if (!MockDictionary.ContainsKey(HttpMessageMock.GetMockKey(value)))
                MockDictionary.Add(HttpMessageMock.GetMockKey(value), value);
            else
                Debug.Write($"[{GetType().Name}] WARNING: A HttpMessageMock was already setup for {string.Join(", ", HttpMessageMock.MockKeyComposition(value))}. The following new entry will be ignored: {value.Describe()}");

            return this;
        }

        public HttpClient Build()
        {
            var handler = new FakeMessageHandler(MockDictionary, DefaultBehavior);
            return new HttpClientMock(handler)
                {
                    BaseAddress = BaseUrl
                };
        }
    }
}
