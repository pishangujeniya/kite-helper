using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace KiteHelper.Domain.HistoricalData
{
    public class HistoricalDataRequestModel
    {
        [JsonPropertyName("Exchange")] public string Exchange { get; set; }
        [JsonPropertyName("TradingSymbol")] public string TradingSymbol { get; set; }
        [JsonPropertyName("StartDateTime")] public DateTime StartDateTime { get; set; }
        [JsonPropertyName("EndDateTime")] public DateTime EndDateTime { get; set; }
        [JsonPropertyName("Interval")] public string Interval { get; set; }
    }
}
