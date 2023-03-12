using System;
using System.Linq;

namespace HttpClientMock
{
    internal static class Helpers
    {
        internal static string DEFAULT_BASE_URL = "https://github.com/";
        public static Uri Combine(this Uri _self, params string[] paths)
            => paths?.Aggregate(
                    _self.AbsoluteUri, 
                    (current, path) => string.Format
                    (
                        "{0}/{1}", 
                        current.TrimEnd('/'),
                        path.TrimStart('/').TrimEnd('/')
                    ))
                .AsUri() ?? _self;

        public static Uri AsUri(this string _self)
            => new Uri(_self);

        public static string Describe(this object _self)
            // ToDo: Print properly an object description
            => _self.ToString();
    }
}
