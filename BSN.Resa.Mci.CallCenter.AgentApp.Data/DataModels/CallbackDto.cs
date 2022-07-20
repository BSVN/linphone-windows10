using System;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels
{
    public class CallbackDto
	{
		protected CallbackDto() { }

		public CallbackDto(string calleeNumber, string callerNumber, long unixTimeSeconds)
		{
			CalleeNumber = calleeNumber;
			CallerNumber = callerNumber;
			Time = unixTimeSeconds;
		}

		public string CalleeNumber { get; private set; }

		public string CallerNumber { get; private set; }

		public long Time { get; private set; }

		public DateTime RequestedAt => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Time);
	}
}
