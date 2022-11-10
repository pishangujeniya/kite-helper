using System.Text.Json.Serialization;

namespace KiteHelper.Infrastructure.Entities
{
    public class KiteInstrumentsEntity
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        
        [JsonPropertyName("InstrumentToken")]
        public string InstrumentToken { get; set; }
        
        [JsonPropertyName("ExchangeToken")]
        public string ExchangeToken { get; set; }
        
        [JsonPropertyName("TradingSymbol")]
        public string TradingSymbol { get; set; }
        
        [JsonPropertyName("Name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("LastPrice")]
        public double? LastPrice { get; set; }
        
        [JsonPropertyName("Expiry")]
        public DateTime? Expiry { get; set; }
        
        [JsonPropertyName("Strike")]
        public string? Strike { get; set; }
        
        [JsonPropertyName("TickSize")]
        public double TickSize { get; set; }
        
        [JsonPropertyName("LotSize")]
        public int LotSize { get; set; }
        
        [JsonPropertyName("InstrumentType")]
        public string InstrumentType { get; set; }
        
        [JsonPropertyName("Segment")]
        public string Segment { get; set; }
        
        [JsonPropertyName("Exchange")]
        public string Exchange { get; set; }
    }

}
