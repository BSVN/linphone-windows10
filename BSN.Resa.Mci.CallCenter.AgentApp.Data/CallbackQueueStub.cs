using BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
	public class CallbackQueueStub : ICallbackQueue
	{
		public CallbackDto Pop()
		{
			return new CallbackDto(number: "1000", rank: DateTimeOffset.UtcNow.ToUnixTimeSeconds());
		}

		public void Push(CallbackDto callback)
		{
		}
	}
}
