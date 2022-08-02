using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketInputModel
    {
        public string CustomerPhoneNumber { get; set; }

        public string Comment { get; set; }

        public Guid TicketTypeId { get; set; }
    }
}
