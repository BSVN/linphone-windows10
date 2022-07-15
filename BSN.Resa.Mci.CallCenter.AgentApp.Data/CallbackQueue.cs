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
		public CallbackQueue(IDatabaseFactory databaseFactory)
		{
			RedisContext redisContext = (RedisContext)databaseFactory.Get();
			database = redisContext.Database;
		}

		public CallbackDto Pop()
		{
			SortedSetEntry? callbackHolder = database?.SortedSetPop(QUEUE_NAME);

			if (callbackHolder == null)
				return null;
			SortedSetEntry callback = callbackHolder.Value;
			return new CallbackDto(number: callback.Element, rank: callback.Score);
		}

		public void Push(CallbackDto callback)
		{
			database?.SortedSetAdd(QUEUE_NAME, callback.Number, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
		}

		private readonly IDatabase database;
		private const string QUEUE_NAME = "cbq";
	}
}
