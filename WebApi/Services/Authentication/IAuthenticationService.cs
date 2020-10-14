using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Helpers;

namespace WebApi.Services.Authentication
{
	public interface IAuthenticationService : IService
	{
		Task<UserSession> RestoreSessionAsync(int sessionId);
	}
}
