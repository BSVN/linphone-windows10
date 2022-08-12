using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketCommandServiceCloseRequest
    {
        public Guid TicketId { get; set; }

        public string Comment { get; set; }
    }
}
