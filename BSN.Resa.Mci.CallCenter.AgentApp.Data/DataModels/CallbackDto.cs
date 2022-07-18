using System;
using System.Text.Json.Serialization;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels
{
    public class CallbackDto
	{
		protected CallbackDto() { }

		public CallbackDto(string calleeNumber, string callerNumber, DateTime time)
		{
			CalleeNumber = calleeNumber;
			CallerNumber = callerNumber;
			Time = time;
		}

		public string CalleeNumber { get; private set; }

		public string CallerNumber { get; private set; }

		[JsonConverter(typeof(CallbackDateTimeConverter))]
		public DateTime Time { get; private set; }
	}
}
