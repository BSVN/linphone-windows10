using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketCommandServiceUpdateRequest
    {
        public Guid TicketId { get; set; }

        public string Comment { get; set; }
    }

}
