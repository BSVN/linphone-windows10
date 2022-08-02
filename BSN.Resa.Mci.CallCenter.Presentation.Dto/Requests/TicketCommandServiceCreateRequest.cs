using BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz;
using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketCommandServiceCreateRequest
    {
        public string CustomerPhoneNumber { get; set; }

        public string Comment { get; set; }

        public Guid TicketTypeId { get; set; }
    }

    public class TicketCommandServiceCreateStoreComplaintRequest
    {
        public string CustomerPhoneNumber { get; set; }

        public string Comment { get; set; }

        public string RelatedNationalCodetoStore { get; set; }

        public string StoreId { get; set; }

        public int Priority { get; set; } = 1;

        public ComplaintType ComplaintType { get; set; }

        public Guid TicketTypeId { get; set; }
    }
}
