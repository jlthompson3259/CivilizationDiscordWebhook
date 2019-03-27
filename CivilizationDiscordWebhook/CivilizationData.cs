using Newtonsoft.Json;

namespace CivilizationDiscordWebhook
{
    public class CivilizationData
    {
        [JsonProperty("value1")]
        public string GameName { get; set; }

        [JsonProperty("value2")]
        public string PlayerName { get; set; }

        [JsonProperty("value3")]
        public string TurnNumber { get; set; }
    }
}
