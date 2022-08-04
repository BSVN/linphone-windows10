using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketTypeViewModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string Code { get; set; }
        
        public WorkgroupViewModel Work { get; set; }
    }
}
