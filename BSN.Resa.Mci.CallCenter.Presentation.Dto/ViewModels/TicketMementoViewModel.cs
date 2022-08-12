using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class TicketMementoViewModel
    {
        public string State { get; set; }

        public string Comment { get; set; }

        public DateTime SubmitedAt { get; set; }

        public DateTime AuditedAt { get; set; }

        public string AssigneeFullName { get; set; }

        public string AuditorFullName { get; set; }

        public string Workgroup { get; set; }
    }
}
