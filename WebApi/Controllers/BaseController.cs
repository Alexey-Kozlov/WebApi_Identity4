using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApi.Models;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    public class BaseController : Controller
    {
        protected object ValidateModelResult(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            string res = null;
            if (!modelState.IsValid)
            {
                res = string.Join("\n", ModelState.Values.Select(v => string.Join("\n", v.Errors.Select(e => e.ErrorMessage))));
            }

            var json = new ErrorData(res);
            return json;
        }

        /// <summary>
        /// информация о пользователе
        /// </summary>
        protected TokenIdentity CurrentUser
        {
            get
            {
                TokenIdentity tokenIdentity = new TokenIdentity(
                    int.Parse(HttpContext.User.Claims.First(claim => claim.Type == Keywords.SessionId).Value),
                    HttpContext.User.Claims.First(claim => claim.Type == Keywords.UserName).Value,
                    HttpContext.User.Claims.First(claim => claim.Type == Keywords.Login).Value,
                    HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    HttpContext.User.Claims.Where(claim => claim.Type == Keywords.Roles).Select(p => p.Value).ToList<string>(),
                    HttpContext.User.Claims
                );
                return tokenIdentity;
            }
        }

        /// <summary>
        /// обобщенный ответ POST метода
        /// </summary>
        protected IActionResult Post<T>(long createId, T request, string userMessage = "")
            => Ok(new PostResult<T>(createId, request, userMessage));

    }
}
