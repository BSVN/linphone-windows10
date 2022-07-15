using BSN.Commons.Infrastructure;
using Serilog;
using StackExchange.Redis;
using System;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
    public class DatabaseFactory : IDatabaseFactory, IDisposable
	{
		public DatabaseFactory(string connectionString)
		{
			_connectionString = connectionString;
			_logger = Log.Logger.ForContext("SourceContext", nameof(DatabaseFactory));
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public IDbContext Get()
		{
			return new RedisContext(Database);
		}

		private ConnectionMultiplexer RedisConnectionMultiplexer
		{
			get
			{
				if (connectionMultiplexer == null)
				{
                    try
                    {
						connectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
                    }
					catch(Exception ex)
                    {
						_logger.Error(ex, "Unable to connect to the Redis server.");
                    }
				}

				return connectionMultiplexer;
			}
		}

		public IDatabase Database
		{
			get
			{
				if (database == null)
				{
					database = RedisConnectionMultiplexer?.GetDatabase();
				}

				return database;
			}
		}

        private ConnectionMultiplexer connectionMultiplexer;
		private IDatabase database;
		private readonly ILogger _logger;

		private readonly string _connectionString;
	}
}
