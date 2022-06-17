using BSN.Commons.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
	public class DatabaseFactory : IDatabaseFactory, IDisposable
	{
		public DatabaseFactory(string connectionString)
		{
			_connectionString = connectionString;
		}
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public IDbContext Get()
		{
			throw new NotImplementedException();
		}

		private ConnectionMultiplexer RedisConnectionMultiplexer
		{
			get
			{
				if (connectionMultiplexer == null)
				{
					connectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
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

		private readonly string _connectionString;
	}
}
