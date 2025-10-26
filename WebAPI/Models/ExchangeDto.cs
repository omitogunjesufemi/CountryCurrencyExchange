using Newtonsoft.Json;

namespace WebAPI.Models
{
    public class ExchangeDto
    {
        [JsonProperty("rates")]
        public Dictionary<string, decimal>? Rates { get; set; }
    }
}
