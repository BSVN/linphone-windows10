using System.Text.Json.Serialization;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Rubika
{
    public class GenericRubikaServiceRequestBase<T> where T : class
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("api_version")]
        public int ApiVersion => 1;

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
