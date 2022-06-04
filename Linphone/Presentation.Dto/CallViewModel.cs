using System;

namespace BelledonneCommunications.Linphone.Presentation.Dto
{
    public class CallViewModel
    {
        public Guid Id { get; set; }

        public string CallerPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }

        public string OperatorFullName { get; set; }

        public Guid OperatorId { get; set; }

        public CallState State { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime FinishedAt { get; set; }

        public TimeSpan Duration { get; set; }

        public Guid? TicketId { get; set; }

        public CallReason? CallReason { get; set; }
    }

    public enum CallState
    {
        Ringing = 1,
        Accepted = 2,
        Missed = 3,
        NormalCleared = 5,
        Terminated = 5
    }

    public enum CallReason
    {
        Callback = 1,
        Question = 2,
        Objection = 3,
        TicketFollowUp = 4,
        Other = 5
    }
}
