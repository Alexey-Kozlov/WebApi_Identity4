using System.Collections.Generic;
using System.Security.Claims;


namespace WebApi.Models
{
    public class TokenIdentity : ClaimsIdentity
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="userName"></param>
		/// <param name="token"></param>
		/// <param name="roles"></param>
		/// <param name="claims"></param>
		public TokenIdentity(int sessionId, string userName, string login, string ipAddress, IEnumerable<string> roles, IEnumerable<Claim> claims)
			: base(claims, "token")
		{
			SessionId = sessionId;
			UserName = userName;
			Login = login;
			ClientIpAddress = ipAddress;
			Roles = roles;
		}

		public int SessionId { get; }
		public string UserName { get; }
		public string ClientIpAddress { get; }
		public string Login { get; }
		public IEnumerable<string> Roles { get; }
	}
}
