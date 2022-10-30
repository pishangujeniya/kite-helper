using KiteHelper.Infrastructure;
using KiteHelper.Infrastructure.Entities;

namespace KiteHelper.Helpers
{
    public static class KiteInstrumentsHelper
    {
        public static async Task LoadKiteInstrumentsCsv(DatabaseContext context)
        {
            // Getting KiteInstrumentsCsv in before starting the application
            var kiteInstrumentsCsv = await KiteConnectSdk.KiteSdk.GetInstrumentsCsv();

            for (int i = 0; i < kiteInstrumentsCsv.Rows.Count; i++)
            {
                var row = kiteInstrumentsCsv.Rows[i];

                var lastPrice = row["last_price"].ToString();
                var expiryDateTime = row["expiry"].ToString();
                var tickSize = row["tick_size"].ToString();
                var lotSize = row["lot_size"].ToString();

                var kiteInstrument = new KiteInstrumentsEntity()
                {
                    InstrumentToken = (string)row["instrument_token"],
                    ExchangeToken = (string)row["exchange_token"],
                    TradingSymbol = (string)row["tradingsymbol"],
                    Name = row["name"].ToString(),
                    LastPrice = !string.IsNullOrWhiteSpace(lastPrice) ? Double.Parse(lastPrice) : null,
                    Expiry = !string.IsNullOrWhiteSpace(expiryDateTime) ? DateTime.Parse(expiryDateTime) : null,
                    Strike = row["strike"].ToString(),
                    TickSize = !string.IsNullOrWhiteSpace(tickSize) ? Double.Parse(tickSize) : 0,
                    LotSize = !string.IsNullOrWhiteSpace(lotSize) ? Int32.Parse(lotSize) : 0,
                    InstrumentType = row["instrument_type"].ToString() ?? string.Empty,
                    Segment = row["segment"].ToString() ?? string.Empty,
                    Exchange = row["exchange"].ToString() ?? string.Empty
                };

                context.KiteInstrumentsEntity.Add(kiteInstrument);
            }

            await context.SaveChangesAsync();

        }
    }
}
