using BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz;
using System;
using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class StoreComplaintTicketViewModel
    {
        public Guid Id { get; set; }

        public int TrackingCode { get; set; }

        public string State { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime AuditedAt { get; set; }

        public string TicketType { get; set; }

        public string AssigneeName { get; set; }

        public Guid? AssigneeId { get; set; }

        public string AuditorFullName { get; set; }

        public string Workgroup { get; set; }

        public OperatorViewModel User { get; set; }

        public string CustomerNationalCode { get; set; }

        public string StoreId { get; set; }
        
        public string BehrouzTrackingCode { get; set; }

        public Guid TicketTypeId { get; set; }

        public IEnumerable<TicketMementoViewModel> Mementos { get; set; }
    }
}
