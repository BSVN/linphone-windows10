namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class CallsCommandServiceSubmitRingingEventRequest
    {
        public string CallerPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }
    }
}
