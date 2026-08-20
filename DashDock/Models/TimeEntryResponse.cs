using System.Text.Json.Serialization;

namespace DashDock.Models
{
    public class TimeEntryResponse
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("t1")]
        public double T1 { get; set; }

        [JsonPropertyName("t2")]
        public double T2 { get; set; }

        [JsonPropertyName("ds")]
        public string? Ds { get; set; }
    }
}