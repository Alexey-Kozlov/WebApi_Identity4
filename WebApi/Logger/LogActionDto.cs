using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Logger
{
    public class LogActionDto
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Sub { get; set; }
        public string IdentitySessionId { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Path { get; set; }
        public string Method { get; set; }
        public string QueryString { get; set; }
        public object Body { get; set; }
        public string StatusCode { get; set; }
        public Exception Exception { get; set; }
    }
}
