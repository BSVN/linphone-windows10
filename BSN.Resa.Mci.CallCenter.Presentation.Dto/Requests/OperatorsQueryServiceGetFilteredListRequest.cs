using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class OperatorsQueryServiceGetFilteredListRequest
    {
        public uint PageNumber { get; set; }

        public uint PageSize { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid? WorkgroupId { get; set; }
    }

}
