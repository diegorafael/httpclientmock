# HttpClientMock
A library to help the setup fake HttpClient instances in order to make it easier to mock this dependency in unit tests

# How to use
The package includes a `HttpClientMockBuilder` class to make its creation as simple as possible.

## Quick start
You can easily setup a "Happy" HttpClientMock as follow:

```
var httpClientMock = new HttpClientMockBuilder()
    .Build();
```

Now as the default behavior is to fake resquest-response flow with a `200 OK` empty content, every single request made through `httpClientMock` will start working.

You can change the default behavior within the following `Behavior` options:

- `Unspecified` - Enforces the use to throw an error when an unconfigured method or route is called;
- `FakeAsSuccess` (default) - Respond an empty (`"{}"`) content with a 200 status code as answer;
- `FakeAsFailure` - Alternates randomly between 4xx and 5xx errors.


