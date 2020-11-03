using System;
using System.Threading.Tasks;
using WebApi.Logger;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters
{
    public class ControllerRequestActionLogger: IAsyncActionFilter
    {
        private readonly IHttpLogHandlerService _httpLogHandlerService;

        public ControllerRequestActionLogger(IHttpLogHandlerService httpLogHandlerService)
        {
            _httpLogHandlerService = httpLogHandlerService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await _httpLogHandlerService.LogRequest(context);
            await next();
        }
    }
}
