using Microsoft.Extensions.Configuration;
using WebApi.DataBaseHelpers;

namespace WebApi.Repositories
{
	public class BaseRepository
	{
		public MSSqlDB Db { get; set; }
		public string ConnectionString { get; private set; }

		public BaseRepository(IConfiguration configuration)
		{
			ConnectionString = configuration.GetConnectionString("TestDb");
			Db = new MSSqlDB(ConnectionString);
		}

		public BaseRepository(string connectionString)
		{
			ConnectionString = connectionString;
			Db = new MSSqlDB(ConnectionString);
		}
		protected MSSqlDB GetDb(string connectionString) => new MSSqlDB(connectionString);
	}
}
