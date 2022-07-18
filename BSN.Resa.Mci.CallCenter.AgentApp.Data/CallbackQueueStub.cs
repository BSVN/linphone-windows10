using BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels;
using System;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
    public class CallbackQueueStub : ICallbackQueue
	{
		public CallbackDto Pop()
		{
			return new CallbackDto(calleeNumber: "99970", callerNumber: "09124569874", time: DateTime.UtcNow);
		}

		public void Push(CallbackDto callback)
		{
		}
	}
}
