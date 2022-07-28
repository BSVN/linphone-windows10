using System;
using System.Text.Json;

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
			
			var callbackDto = JsonSerializer.Deserialize<CallbackDto>(callback.Element.ToString());

			double tryCount;

			var tryCountEntry = database.HashGet("cbq-trycount", $"{callbackDto.callee_number}*{callbackDto.caller_number}");
			if (tryCountEntry.HasValue)
			{
				tryCountEntry.TryParse(out tryCount);
				callbackDto.try_count = (int)tryCount;
			}
			else
			{
				callbackDto.try_count = 0;
			}

			callbackDto.time = callback.Score;

			return callbackDto;
		}

		public void Push(CallbackDto callback)
		{
			if (callback.try_count < 3)
            {
				database?.SortedSetAdd(QUEUE_NAME, JsonSerializer.Serialize(callback), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
				database.HashSet("cbq-trycount", $"{callback.callee_number}*{callback.caller_number}", callback.try_count);
			}
			else
            {
				database.HashDelete("cbq-trycount", $"{callback.callee_number}*{callback.caller_number}");
			}
		}

		private readonly IDatabase database;
		private const string QUEUE_NAME = "cbq";
	}
}
