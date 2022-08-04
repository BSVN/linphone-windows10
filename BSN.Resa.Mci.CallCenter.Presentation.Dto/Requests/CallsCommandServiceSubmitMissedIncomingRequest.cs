namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsCommandServiceSubmitMissedIncomingRequest
    {
        public string CallerPhoneNumber { get; set; }

        public string AgentPhoneNumber { get; set; }

        public string InboundService { get; set; }
    }
}
