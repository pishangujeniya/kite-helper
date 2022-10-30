namespace KiteHelper.Infrastructure.Entities
{
    public class KiteInstrumentsEntity
    {
        public int Id { get; set; }
        public string InstrumentToken { get; set; }
        public string ExchangeToken { get; set; }
        public string TradingSymbol { get; set; }
        public string? Name { get; set; }
        public double? LastPrice { get; set; }
        public DateTime? Expiry { get; set; }
        public string? Strike { get; set; }
        public double TickSize { get; set; }
        public int LotSize { get; set; }
        public string InstrumentType { get; set; }
        public string Segment { get; set; }
        public string Exchange { get; set; }
    }
}
