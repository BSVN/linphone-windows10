using System;

namespace BelledonneCommunications.Linphone.Presentation.Dto
{
    public class CallCampaignShortViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime StartsAt { set; get; }

        public DateTime EndsAt { set; get; }

        public CampaignState State { get; set; }
    }

    public enum CampaignState
    {
        Running = 1,
        Suspended = 2,
        Failed = 3,
        Finished = 4,
        Stopped = 5
    }
}
