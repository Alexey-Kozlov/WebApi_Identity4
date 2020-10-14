using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : BaseController
    {
        [HttpGet]
        public IActionResult Get()
        {
            var user = CurrentUser;
            return Ok(user.SessionId);

        }
    }
}
