using BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels;
using System;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
    public class CallbackQueueStub : ICallbackQueue
	{
		public CallbackDto Pop()
		{
			return new CallbackDto(calleeNumber: "99970", callerNumber: "009357277978", unixTimeSeconds: ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
		}

		public void Push(CallbackDto callback)
		{
		}
	}
}
