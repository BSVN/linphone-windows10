using System;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels
{
    public class CallbackDto
	{
		protected CallbackDto() { }

		public CallbackDto(string callee_number, string caller_number, string time)
		{
			this.callee_number = callee_number;
			this.caller_number = caller_number;
			this.time = time;
		}

		public string callee_number { get; private set; }

		public string caller_number { get; private set; }

		public string time { get; private set; }

		public DateTime RequestedAt => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(time));
	}
}
