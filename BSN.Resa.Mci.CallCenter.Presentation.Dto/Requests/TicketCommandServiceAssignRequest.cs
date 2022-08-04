using System;
using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketCommandServiceAssignRequest
    {
        public List<Guid> TicketIds { get; set; }

        public Guid AssigningUserId { get; set; }

        public string Comment { get; set; }
    }
}
