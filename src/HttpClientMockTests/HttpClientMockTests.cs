using FluentAssertions;
using HttpClientMock;
using Newtonsoft.Json;
using System.Net;

namespace HttpClientMockTests
{
    public class HttpClientMockTests
    {
        [Theory]
        [MemberData(nameof(RouteMocks))]
        public async Task Should_respond_with_mocked_object_to_a_configured_route(string route, object expectedResponseObject)
        {
            var httpFake = new HttpClientMockBuilder()
                .Mocking(route, expectedResponseObject)
                .Build();

            var res = await httpFake.GetAsync(route);
            var actualResponseObject = JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync(), expectedResponseObject.GetType());
            
            res.IsSuccessStatusCode.Should().BeTrue();
            actualResponseObject.Should().NotBeNull();
            actualResponseObject.Should().BeEquivalentTo(expectedResponseObject);
        }

        [Fact]
        public async Task Should_respond_a_http_error_to_non_specified_route_requested_when_default_behavior_is_FakeAsFailure()
        {
            var httpFake = new HttpClientMockBuilder()
                .WithDefaultBehavior(Behavior.FakeAsFailure)
                .Build();

            var res = await httpFake.GetAsync("any/route");

            res.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Should_respond_a_200_ok_to_non_specified_route_requested_when_default_behavior_is_FakeAsSuccess()
        {
            var httpFake = new HttpClientMockBuilder()
                .WithDefaultBehavior(Behavior.FakeAsSuccess)
                .Build();

            var res = await httpFake.GetAsync("any/route");

            res.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_throw_an_error_when_request_a_non_specified_route_when_default_behavior_is_Unspecified()
        {
            var httpFake = new HttpClientMockBuilder()
                .WithDefaultBehavior(Behavior.Unspecified)
                .Build();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>  await httpFake.GetAsync("any/route"));
        }

        public static IEnumerable<object[]> RouteMocks = new[]
        {
            new object [] { "path", new { Id = 1, Name = "sample" } },
            new object [] { "path/sub/route", new { Number = 42, Description = "the answer", InnerObject = new { Name = "Child" } } },
            new object [] { "/sub/site/", new { Number = 333, InnerObject = new { Id = 9, Name = "test" } } },
            new object [] { "api/", new { Message = "Ok" } },
            new object [] { "/api", new { Message = "Error" } },
        };
    }
}