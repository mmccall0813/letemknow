using System.Text.Json.Serialization;

namespace macrottie.letemknow;

public class Config {
    [JsonInclude] public bool UseLastFm = false;
    [JsonInclude] public string LastFmUsername = "";
    [JsonInclude] public string LastFmApikey = "";
}
