using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Serilog;
using System.Reflection;
using System.Text;

namespace WebApi.Logger
{
    public class HttpLogHandlerService : IHttpLogHandlerService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpLogHandlerService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public Task LogRequest(ActionExecutingContext context)
        {
            context.HttpContext.Request.EnableBuffering();
            var headers = context.HttpContext.Request.Headers.ToDictionary(header => header.Key,
                                          header => header.Value.ToString(), StringComparer.OrdinalIgnoreCase);

            var headerVal = headers.SingleOrDefault(h => h.Key == "Authorization");
            var sessionId = string.Empty;
            var sub = string.Empty;
            if (headerVal.Value != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenValue = headerVal.Value.Replace("Bearer ", string.Empty);
                if (tokenHandler.CanReadToken(tokenValue) && tokenHandler.ReadToken(tokenValue) is JwtSecurityToken jwtToken)
                {
                    sessionId = jwtToken.Claims.SingleOrDefault(claim => claim.Type == "sessionId")?.Value;
                    sub = jwtToken.Claims.SingleOrDefault(claim => claim.Type == "sub")?.Value;

                    headers.Remove("Authorization");
                }
            }

            var requestModel = new LogActionDto
            {
                Scheme = context.HttpContext.Request.Scheme,
                Host = context.HttpContext.Request.Host.Host,
                IdentitySessionId = sessionId,
                Sub = sub,
                Path = context.HttpContext.Request.Path,
                Headers = headers,
                Method = context.HttpContext.Request.Method,
                QueryString = $"{context.HttpContext.Request.QueryString}",
                Body = GetActionArguments(context.ActionArguments),
                StatusCode = $"{context.HttpContext.Response.StatusCode}",
            };

            Log.Information("{@Log}{@request}", new { Type = LogActionType.Request.ToString() }, requestModel);

            return Task.CompletedTask;
        }

        public Task LogResponse(string body)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var headers = httpContext.Request.Headers.ToDictionary(header => header.Key,
                header => header.Value.ToString(), StringComparer.OrdinalIgnoreCase);
            var (sessionId, sub) = GetTokenValues(headers);

            var responseModel = new LogActionDto
            {
                Scheme = httpContext.Request.Scheme,
                Host = httpContext.Request.Host.Host,
                IdentitySessionId = sessionId,
                Sub = sub,
                Path = httpContext.Request.Path,
                Headers = headers,
                Method = httpContext.Request.Method,
                QueryString = $"{httpContext.Request.QueryString}",
                Body = body,
                StatusCode = $"{httpContext.Response.StatusCode}",
            };
            Log.Information("{@Log}{@response}", new { Type = LogActionType.Response.ToString() }, responseModel);
            return Task.CompletedTask;
        }

        public Task LogException(Exception ex, ExceptionLevel level)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Request.EnableBuffering();
            var body = ReadBody(httpContext.Request.Body).GetAwaiter().GetResult();
            var headers = httpContext.Request.Headers.ToDictionary(header => header.Key,
                header => header.Value.ToString(), StringComparer.OrdinalIgnoreCase);
            var (sessionId, sub) = GetTokenValues(headers);

            var exception = new LogActionDto
            {
                Scheme = httpContext.Request.Scheme,
                Host = httpContext.Request.Host.Host,
                IdentitySessionId = sessionId,
                Sub = sub,
                Path = httpContext.Request.Path,
                Headers = headers,
                Method = httpContext.Request.Method,
                QueryString = $"{httpContext.Request.QueryString}",
                Body = body,
                StatusCode = $"500",
                Exception = ex
            };

            switch (level)
            {
                case ExceptionLevel.Warning:
                    Log.Warning("{@Log}{@Warning}", new { Type = LogActionType.Warning.ToString() }, exception);
                    break;
                case ExceptionLevel.Error:
                    Log.Error("{@Log}{@Error}", new { Type = LogActionType.Error.ToString() }, exception);
                    break;
                case ExceptionLevel.Fatal:
                    Log.Error("{@Log}{@Fatal}", new { Type = LogActionType.Fatal.ToString() }, exception);
                    break;
            }

            return Task.CompletedTask;
        }

        private Dictionary<string, string> GetActionArguments(IDictionary<string, object> arguments)
        {
            var logArgs = new Dictionary<string, string>();
            //исключаем поля класса, помеченного атрибутом LogLessAttribute. В этом атрибуте перечислены поля, запрещенные отображаться в логе
            foreach (var keyValue in arguments)
            {
                var argType = keyValue.Value.GetType();
                var attributes = argType.GetCustomAttributes(typeof(LogLessAttribute),true);
                foreach (LogLessAttribute attr in attributes)
                {                    
                    var props = argType.GetProperties()
                                       .Where(p => !attr.Properties.Contains(p.Name))
                                       .ToArray();
                    StringBuilder rez = new StringBuilder();
                    foreach(PropertyInfo info in props)
                    {
                        rez.Append(info.Name + ":" + JsonConvert.SerializeObject(info.GetValue(keyValue.Value)) + ";");
                    }

                    logArgs.Add(keyValue.Key, rez.ToString());
                }
                if(attributes.Length == 0)
                {
                    logArgs.Add(keyValue.Key, JsonConvert.SerializeObject(keyValue.Value));
                }
            }

            return logArgs;
        }

        private (string sessionId, string sub) GetTokenValues(Dictionary<string, string> headers)
        {
            var header = headers.SingleOrDefault(h => h.Key == "Authorization");
            var sessionId = "";
            var sub = "";
            if (header.Value != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenValue = header.Value.Replace("Bearer ", string.Empty);
                if (tokenHandler.CanReadToken(tokenValue) && tokenHandler.ReadToken(tokenValue) is JwtSecurityToken jwtToken)
                {
                    sessionId = jwtToken.Claims.SingleOrDefault(claim => claim.Type == "sessionId")?.Value;
                    sub = jwtToken.Claims.SingleOrDefault(claim => claim.Type == "sub")?.Value;
                    headers.Remove("Authorization");
                }
            }
            return (sessionId, sub);
        }

        private async Task<string> ReadBody(Stream body)
        {
            body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(body).ReadToEndAsync();
            body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }
    }
}
