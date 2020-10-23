using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.HttpSys;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    public class IdentityController : BaseController
    {
        private readonly ILogger _logger;

        public IdentityController(ILogger<IdentityController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Admin")]
        [Authorize("Admin")]
        public IActionResult GetAdmin()
        {
            _logger.LogInformation($"WebApi - GetAdmin executed.");
            var user = CurrentUser;
            return Ok(user.SessionId);
        }
        [HttpGet("User")]
        [Authorize("User")]
        public IActionResult GetUser()
        {
            var user = CurrentUser;
            return Ok(user.SessionId);

        }
        [HttpGet("Test")]
        [Authorize]
        public IActionResult GetTest()
        {
            if (HttpContext.User.Claims.Any(p => p.Type == "myClaim"))
                return Ok(HttpContext.User.Claims.Where(p => p.Type == "myClaim").FirstOrDefault().Value);
            return Ok("sfsfsd");

        }
    }
}
