using BSN.Resa.Mci.CallCenter.Presentation.Dto;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class AttachCallReasonInputModel
    {
        public string CustomerPhoneNumber { get; set; }

        public CallReason CallReason { get; set; }
    }
}
