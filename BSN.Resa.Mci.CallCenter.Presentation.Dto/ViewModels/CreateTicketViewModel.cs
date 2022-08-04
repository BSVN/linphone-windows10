using System;
using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    // TODO [Noei]: Mr.Mirzaei pls clarify the usage if this shit.
    public class CreateTicketViewModel : TicketInputModel
    {
        public IEnumerable<TicketTypeViewModel> TicketTypes { get; set; }
    }
}
