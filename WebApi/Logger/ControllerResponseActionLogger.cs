using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebApi.Logger
{
    public class ControllerResponseActionLogger
    {
        private readonly RequestDelegate _next;
        private readonly IHttpLogHandlerService _logHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerResponseActionLogger"/> class.
        /// </summary>
        public ControllerResponseActionLogger(RequestDelegate next, IHttpLogHandlerService logHandler)
        {
            _next = next;
            _logHandler = logHandler;
        }

        /// <summary>
        /// Логирование запроса/ответа
        /// </summary>
        /// <param name="context"></param>
        public async Task Invoke(HttpContext context)
        {

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await _next(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await _logHandler.LogResponse(await new StreamReader(context.Response.Body).ReadToEndAsync());
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }

        }
    }
}
