using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsCommandServiceInitiateCallbackRequest
    {
        public string AgentPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }

        public string InboundService { get; set; }

        public DateTime RequestedAt { get; set; }
    }
}
