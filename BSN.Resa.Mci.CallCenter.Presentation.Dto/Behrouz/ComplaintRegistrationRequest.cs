using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz
{
    public class ComplaintRegistrationRequest
    {
        public string TK { get; set; } = "TFGD34";

        public DateTime RelatedDateTime { get; set; } //"1401/03/01T10:23",

        public int ComplaintType { get; set; }

        public int Priority { get; private set; } = 1;

        public string MobileNumberOfComplainer { get; set; }

        public string RelatedNationalCodetoStore { get; set; }

        public string Description { get; set; }
    }
}
