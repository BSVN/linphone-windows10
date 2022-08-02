using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsCommandServiceAttachTicketRequest
    {
        public Guid CallId { get; set; }

        public Guid TicketId { get; set; }
    }

    public class CallsCommandServiceEditCallReasonRequest
    {
        public Guid CallId { get; set; }

        public CallReason CallReason { get; set; }

    }
}
