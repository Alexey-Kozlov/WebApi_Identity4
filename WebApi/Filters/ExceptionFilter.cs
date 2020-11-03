using System;
using System.Net;
using WebApi.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace WebApi.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly IHttpLogHandlerService _logHandler;
        private const string InternalErrorText = "По техническим причинам операция не была исполнена. Попробуйте повторить позднее.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
        /// </summary>
        /// <param name="exceptionHandlerLogger">The exception handler logger.</param>
        /// <param name="logHandler"></param>
        public ExceptionFilter(IHttpLogHandlerService logHandler)
        {
            _logHandler = logHandler;
        }
        /// <summary>
        /// Called after an action has thrown an <see cref="T:System.Exception" />.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" />.</param>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception == null)
            {
                return;
            }

            int resultStatusCode = (int)HttpStatusCode.InternalServerError;
            string resultMessage = InternalErrorText;
            if (context.Exception is MessageException)
            {
                _logHandler.LogException(context.Exception, ExceptionLevel.Warning);
                resultStatusCode = (int)HttpStatusCode.BadRequest;
                resultMessage = context.Exception.Message;
            }
            else
            {
                _logHandler.LogException(context.Exception, ExceptionLevel.Error);
            }
            context.Result = new ContentResult
            {
                StatusCode = resultStatusCode,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(new { Message = resultMessage })
            };
            context.ExceptionHandled = true;
        }
    }
}
