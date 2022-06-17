using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
	using BSN.Commons.Infrastructure;
	using DataModels;
	using StackExchange.Redis;

	public interface ICallbackQueue
	{
		CallbackDto Pop();
		void Push(CallbackDto callback);
	}

	public class CallbackQueue : ICallbackQueue
	{
		public CallbackQueue(DatabaseFactory databaseFactory)
		{
			database = databaseFactory.Database;
		}

		public CallbackDto Pop()
		{
			SortedSetEntry? callback = database.SortedSetPop(QUEUE_NAME);
			return new CallbackDto(number: callback?.Element);
		}

		public void Push(CallbackDto callback)
		{
			database.SortedSetAdd(QUEUE_NAME, callback.Number, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
		}

		private readonly IDatabase database;
		private const string QUEUE_NAME = "cbq";
	}
}
