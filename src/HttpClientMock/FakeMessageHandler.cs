using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientMock
{
    internal class FakeMessageHandler : HttpMessageHandler
    {
        private readonly IDictionary<string, HttpMessageMock> ConfiguredMockResponses;
        private readonly Behavior DefaultBehavior;

        public FakeMessageHandler(IDictionary<string, HttpMessageMock> configuredMockResponses, Behavior defaultBehavior)
        {
            ConfiguredMockResponses = configuredMockResponses;
            DefaultBehavior = defaultBehavior;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var mockedResponse = CreateResponseFor(request);

            return await Task.FromResult(mockedResponse);
        }

        private HttpResponseMessage CreateResponseFor(HttpRequestMessage request)
        {
            if (ConfiguredMockResponses.TryGetValue(HttpMessageMock.GetMockKey(request), out HttpMessageMock mock))
                return mock.AsHttpResponseMessage();
            else
                switch (DefaultBehavior)
                {
                    case Behavior.Unspecified:
                        throw new InvalidOperationException($"The specified request has no mock response configured");
                    case Behavior.FakeAsSuccess:
                        return HttpMessageMock.Success(request);
                    case Behavior.FakeAsFailure:
                        return HttpMessageMock.Fail(request);
                }
            throw new NotImplementedException();
        }
    }
}
