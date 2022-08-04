using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketAssignInputModel
    {
        public Guid TicketId { get; set; }

        public Guid AssigningUserId { get; set; }

        public string Comment { get; set; }
    }
}
