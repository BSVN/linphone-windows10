using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallbackRequestViewModel
    {
        public string CalleePhoneNumber { get; set; }

        public string CallerPhoneNumber { get; set; }

        public DateTime RequestedAt { get; set; }

        public int CallTryCount { get; set; }
    }
}
