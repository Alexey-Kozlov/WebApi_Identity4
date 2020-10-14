using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Models;
using WebApi.Helpers;
using WebApi.Repositories.Users;

namespace WebApi.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IConfiguration _config;

		public AuthenticationService(IServiceProvider serviceProvider, IConfiguration config)
		{
			_serviceProvider = serviceProvider;
			_config = config;
		}

		public async Task<UserSession> RestoreSessionAsync(int sessionId)
		{
			var repository = GetRepository();
			return await repository.FindSessionAsync(sessionId);
		}

		private IUserRepository GetRepository()
		{
			var services = _serviceProvider.GetServices<IUserRepository>();
			return services.First();
		}
	}
}
