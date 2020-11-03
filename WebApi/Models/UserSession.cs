using System.Collections.Generic;
using WebApi.Logger;

namespace WebApi.Models
{
	public class UserSession
	{
		public string UserName { get; set; }
		public string LoginName { get; set; }
		public int UserId { get; set; }
		public int SessionId { get; set; }
		public IList<string> Roles { get; set; } = new List<string>();
		public string Message { get; set; }

		public override string ToString()
		{
			return $"UserName  = {UserName}, SessionId = {SessionId}";
		}
	}
}
