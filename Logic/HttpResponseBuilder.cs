using System.Collections.Generic;
using System.Text;
using Logic.Interfaces;

namespace Logic
{
    public class HttpResponseBuilder : IHttpResponseBuilder
    {
        public string Build(int code, string status,  IReadOnlyDictionary<string, string> headers, string body)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"HTTP/1.1 {status} {status}");

            foreach (var (key, value) in headers)
            {
                sb.AppendLine($"{key}: {value}");
            }

            sb.AppendLine();
            sb.AppendLine(body);
            return sb.ToString();
        }
    }
}