using System.Net.Http;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("HttpClientMockTests")]
namespace HttpClientMock
{
    public class HttpClientMock : HttpClient
    {
        internal HttpClientMock(HttpMessageHandler handler) : base(handler)
        {

        }
    }
}
