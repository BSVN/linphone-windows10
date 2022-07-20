using BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels;
using System;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
    public class CallbackQueueStub : ICallbackQueue
	{
		public CallbackDto Pop()
		{
			return new CallbackDto(callee_number: "99970", caller_number: "009357277978", time: ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString());
		}

		public void Push(CallbackDto callback)
		{
		}
	}
}
