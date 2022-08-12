namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsCountByStateViewModel
    {
        public CallState State { get; set; }

        public string InboundService { get; set; }

        public int IncomingCallCount { get; set; }

        public int OutgoingCallCount { get; set; }

        public int CallbackCount { get; set; }

        public int Total { get; set; }
    }
}
