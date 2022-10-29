using Microsoft.AspNetCore.Mvc;

namespace KiteHelper.Domain.TradingSymbols
{
    public class TradingSymbolsRequestModel
    {
        [FromQuery(Name = "exchange")] public string exchange { get; set; }
        [FromQuery(Name = "tradingSymbol")] public string tradingSymbol { get; set; }
    }
}
