using BSN.Commons.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
	public class RedisContext : IDbContext
	{
		public RedisContext(IDatabase database)
		{
			Database = database;
		}

		public IDatabase Database { get; set; }

		public int SaveChanges()
		{
			throw new NotImplementedException();
		}
	}
}
