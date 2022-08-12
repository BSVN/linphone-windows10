using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketCommandServiceForwardRequest
    {
        public Guid TicketId { get; set; }

        public Guid DestinationWorkgroupId { get; set; }

        public string Comment { get; set; }
    }
}
