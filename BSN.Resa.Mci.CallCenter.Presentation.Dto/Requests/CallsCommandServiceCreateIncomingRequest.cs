namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsCommandServiceInitiateIncomingRequest
    {
        public string CallerPhoneNumber { get; set; }

        public string AgentPhoneNumber { get; set; }

        public string InboundService { get; set; }
    }
}
