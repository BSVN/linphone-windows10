using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallbackViewModel
    {
        public Guid Id { get; set; }

        public string CallerPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }

        public string InboundService { get; set; }

        public string OperatorFullName { get; set; }

        public Guid OperatorId { get; set; }

        public CallState State { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime FinishedAt { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime RequestedAt { get; set; }

        public Guid? TicketId { get; set; }

        public CallReason? CallReason { get; set; }

        public CallType CallType { get; set; }
    }
}
