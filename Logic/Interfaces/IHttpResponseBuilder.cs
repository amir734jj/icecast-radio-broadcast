using System.Collections.Generic;

namespace Logic.Interfaces
{
    public interface IHttpResponseBuilder
    {
        string Build(int code, string status, IReadOnlyDictionary<string, string> headers, string body);
    }
}