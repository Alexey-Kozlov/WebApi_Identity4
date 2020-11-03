using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Logger
{
    public interface IHttpLogHandlerService
    {
        Task LogRequest(ActionExecutingContext context);
        Task LogResponse(string body);
        Task LogException(Exception ex, ExceptionLevel level);
    }
}
