using BSN.Commons.Infrastructure;
using Serilog;
using StackExchange.Redis;
using System;
using System.Threading;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
	public class DatabaseFactory : IDatabaseFactory, IDisposable
	{
		public delegate void RedisConnectionEstablished(ConnectionMultiplexer connection);
		public event RedisConnectionEstablished OnConnectionEstablished;

		public DatabaseFactory(string connectionString)
		{
			_connectionString = connectionString;
			_logger = Log.Logger.ForContext("SourceContext", nameof(DatabaseFactory));
		}

		public void Dispose()
		{

		}

		public IDbContext Get()
		{
			return new RedisContext(Database);
		}

		private ConnectionMultiplexer RedisConnectionMultiplexer
		{
			get
			{
				if (_connectionMultiplexer == null)
				{
					try
					{
						_connectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
					}
					catch (Exception ex)
					{
						_timer = new Timer(Reconnect, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
					}
				}

				return _connectionMultiplexer;
			}
		}

		public IDatabase Database
		{
			get
			{
				if (_database == null)
				{
					_database = RedisConnectionMultiplexer?.GetDatabase();
				}

				return _database;
			}
		}

		private void Reconnect(object state)
		{
			try
			{
				_connectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
				_timer?.Dispose();
				OnConnectionEstablished?.Invoke(_connectionMultiplexer);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Unable to connect to the Redis server.");
			}
		}

		private ConnectionMultiplexer _connectionMultiplexer;
		private IDatabase _database;
		private Timer _timer;

		private readonly ILogger _logger;

		private readonly string _connectionString;
	}
}
