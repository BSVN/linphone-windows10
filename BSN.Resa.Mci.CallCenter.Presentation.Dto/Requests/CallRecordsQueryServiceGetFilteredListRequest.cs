using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallRecordsQueryServiceGetFilteredListRequest
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
        
        public Guid? AgentId { get; set; }
        
        public Guid? TicketTypeId { get; set; }
        
        public string CallerPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public string InboundService { get; set; }

        public CallReason? CallReason { get; set; }

        public CallState? State { get; set; }

        public uint PageNumber { get; set; }
        
        public uint PageSize { get; set; }
    }
}
