using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallRespondingAgentsQueryServiceGetFilteredListRequest
    {
        public AgentStatus? Status { get; set; }

        public Guid? WorkGroupId { get; set; }

        public uint PageNumber { get; set; }

        public uint PageSize { get; set; }
    }
}
