using System.Text.Json.Serialization;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Rubika
{
    public class GetHouseholdHeadInfoRequest
    {
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
    }
}
