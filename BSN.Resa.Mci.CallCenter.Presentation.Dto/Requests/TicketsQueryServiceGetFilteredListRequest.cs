using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketsQueryServiceGetFilteredListRequest
    {
        public uint PageNumber { get; set; }

        public uint PageSize { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string CustomerPhoneNumber { get; set; }
        
        public int? TrackingCode { get; set; }

        public TicketState? State { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? AssigneeId { get; set; }

        public Guid? WorkGroupId { get; set; }

        public Guid? TicketTypeId { get; set; }
    }
}
