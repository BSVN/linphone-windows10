using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallRecordsCommandServiceAttachCallReasonRequest
    {
        public Guid CallId { get; set; }

        public CallReason CallReason { get; set; }
    }
}
