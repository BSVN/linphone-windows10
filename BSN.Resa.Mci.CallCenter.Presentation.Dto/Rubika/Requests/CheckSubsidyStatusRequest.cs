using System.Text.Json.Serialization;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Rubika
{
    public class CheckSubsidyStatusRequest
    {
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
    }
}
