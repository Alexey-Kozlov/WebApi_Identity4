using System.Threading.Tasks;
using WebApi.Repositories.ReaderFunctions;
using WebApi.Models;
using System.Data;
using WebApi.DataBaseHelpers;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System;

namespace WebApi.Repositories.Users
{
    public class UserRepository :BaseRepository, IUserRepository
    {
        private readonly UserReaders _userReaders;
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
            _userReaders = new UserReaders();
            _configuration = configuration;
        }

        public UserRepository(string connectionString) : base(connectionString)
        { }

        public async Task<UserSession> FindSessionAsync(int sessionId)
        {
            var cmd = Db.CreateProcedureCommand("dbo.SessionCheck");
            cmd.Parameters.AddWithValue("@SessionId", sessionId);
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@LoginName", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
            await Db.ExecuteNonQueryAsync(cmd, CancellationToken.None);
            if(!cmd.ReadOutputValue<int?>("@UserId").HasValue)
            {
                throw new Exception($"Session {sessionId.ToString()} not found");
            }

            return new UserSession
            {
                UserId = cmd.ReadOutputValue<int?>("@UserId").Value,
                UserName = cmd.ReadOutputValue<string>("@UserName"),
                LoginName = cmd.ReadOutputValue<string>("@LoginName"),
                SessionId = sessionId
            };
        }

        public async Task<UserSession> LoginAsync(string login, string password)
        {
            var cmd = Db.CreateProcedureCommand("dbo.Login");
            cmd.Parameters.AddWithValue("@Login", login);
            cmd.Parameters.AddWithValue("@Password", password);
            var userSession = await Db.ExecuteReaderAsync(_userReaders.ReadUserSession, cmd, CancellationToken.None);
            return userSession.FirstOrDefault();
        }

    }
}
