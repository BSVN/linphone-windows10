using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsQueryServiceGeneratePeriodicCallsReportRequest
    {
        public uint Period { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public uint PageNumber { get; set; }

        public uint PageSize { get; set; }
    }
}
