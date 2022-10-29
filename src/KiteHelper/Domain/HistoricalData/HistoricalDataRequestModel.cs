using Microsoft.AspNetCore.Mvc;

namespace KiteHelper.Domain.HistoricalData
{
    public class HistoricalDataRequestModel
    {
        [FromQuery(Name = "exchange")] public string exchange { get; set; }
        [FromQuery(Name = "tradingSymbol")] public string tradingSymbol { get; set; }
        [FromQuery(Name = "startDateTime")] public DateTime startDateTime { get; set; }
        [FromQuery(Name = "endDateTime")] public DateTime endDateTime { get; set; }
        [FromQuery(Name = "interval")] public string interval { get; set; }
    }
}
