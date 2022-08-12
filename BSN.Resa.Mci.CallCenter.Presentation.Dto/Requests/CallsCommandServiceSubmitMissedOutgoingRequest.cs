namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsCommandServiceSubmitMissedOutgoingRequest
    {
        public string AgentPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }

        public string InboundService { get; set; }
    }
}
