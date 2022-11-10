using Microsoft.AspNetCore.Mvc;

namespace KiteHelper.Domain.TradingSymbols
{
    public class TradingSymbolsRequestModel
    {
        [FromQuery(Name = "TradingSymbol")] public string TradingSymbol { get; set; }
    }
}
