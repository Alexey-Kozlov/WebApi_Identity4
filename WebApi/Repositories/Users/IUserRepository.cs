using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Repositories.Users
{
    public interface IUserRepository
    {
        Task<UserSession> FindSessionAsync(int sessionId);
        Task<UserSession> LoginAsync(string login, string password);
    }
}
