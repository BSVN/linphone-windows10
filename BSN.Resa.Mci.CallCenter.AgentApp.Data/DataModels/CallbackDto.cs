using System;
using System.Text.Json.Serialization;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels
{
    public class CallbackDto
	{
		protected CallbackDto() { }

        public CallbackDto(string callee_number, string caller_number)
        {
            this.callee_number = callee_number;
            this.caller_number = caller_number;
        }

        public string callee_number { get; private set; }

        public string caller_number { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double time { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int try_count { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime RequestedAt => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time);
    }
}
