namespace HttpClientMock
{
    public enum ContentFormat
    {
        JsonObject,
        ByteStream
    }

    public enum Behavior
    {
        Unspecified = 0,
        FakeAsSuccess = 1,
        FakeAsFailure = 2
    }
}
