using BSN.Resa.Vns.Commons.Responses;
using System;
using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsQueryServiceGeneratePeriodicCallsReportResponse : GenericResponseBaseWithPagination<CallsPeriodicReportCollectionViewModel> { }

    public class CallsPeriodicReportCollectionViewModel
    {
        public IEnumerable<CallsPeriodicReportViewModel> Items { get; set; }
    }

    public class CallsPeriodicReportViewModel
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public IEnumerable<CallsCountByInboundService> CallsCountByInboundService { get; set; }

        public IEnumerable<CallsCountByStateViewModel> CallsCountByState { get; set; }

        public IEnumerable<CallsCountByCallReasonViewModel> CallsCountByCallReason { get; set; }
    }

    public class CallsCountByInboundService
    {
        public string InboundService { get; set; }

        public int IncomingCallCount { get; set; }

        public int OutgoingCallCount { get; set; }

        public int CallbackCount { get; set; }

        public int Total { get; set; }
    }

    public class CallsCountByCallReasonViewModel
    {
        public CallReason? CallReason { get; set; }

        public int IncomingCallCount { get; set; }

        public int OutgoingCallCount { get; set; }

        public int CallbackCount { get; set; }

        public int Total { get; set; }
    }
}
