using System.Data;
using System.Net;
using KiteConnect;
using KiteConnectSdk;
using KiteHelper.Attributes;
using KiteHelper.Domain.HistoricalData;
using KiteHelper.Domain.Login;
using KiteHelper.Domain.TradingSymbols;
using KiteHelper.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KiteHelper.Controllers
{
    [ApiController]
    [Route("api/kite")]
    public class KiteController : ControllerBase
    {
        private ILogger<KiteController> _logger;

        public KiteController(ILogger<KiteController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Login([FromBody] KiteLoginRequestModel kiteLoginRequestModel)
        {
            try
            {
                KiteSdk kiteSdk = new KiteSdk();
                var isLoggedIn = kiteSdk.Login(kiteLoginRequestModel.UserName, kiteLoginRequestModel.Password, kiteLoginRequestModel.AppCode.ToString());
                if (isLoggedIn)
                {
                    string? sessionId = KiteSessionHelper.AddKiteSession(kiteLoginRequestModel.UserName, kiteSdk);
                    if (!string.IsNullOrWhiteSpace(sessionId))
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return new JsonResult(sessionId);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Login");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("profile")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(Profile))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [KiteAuthorize]
        public async Task<ActionResult> Profile()
        {
            var kiteSdk = KiteSessionHelper.GetKiteSdkFromSession(Request.Headers.Authorization);
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(kiteSdk?.GetProfile());
        }

        [HttpGet]
        [Route("trading-symbols")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(List<string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> TradingSymbols([FromQuery()] TradingSymbolsRequestModel tradingSymbolsRequestModel)
        {
            tradingSymbolsRequestModel.exchange = tradingSymbolsRequestModel.exchange?.ToUpper() ?? string.Empty;
            tradingSymbolsRequestModel.tradingSymbol = tradingSymbolsRequestModel.tradingSymbol?.ToUpper() ?? string.Empty;
            var result = KiteInstruments.KiteInstrumentsCsv?
                .AsEnumerable()
                .Where(myRow => string.IsNullOrWhiteSpace(tradingSymbolsRequestModel.exchange) || (myRow.Field<string>("exchange")?.ToUpper() == tradingSymbolsRequestModel.exchange))
                .Where(myRow => string.IsNullOrWhiteSpace(tradingSymbolsRequestModel.tradingSymbol) || (myRow.Field<string>("tradingsymbol")?.ToUpper().Contains(tradingSymbolsRequestModel.tradingSymbol) ?? false))
                .Select(myRow => myRow.Field<string>("tradingsymbol") ?? string.Empty).ToList();
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(result);
        }

        [HttpGet]
        [Route("historical-data")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(List<Historical>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [KiteAuthorize]
        public async Task<ActionResult> HistoricalData([FromQuery] HistoricalDataRequestModel historicalDataRequestModel)
        {
            historicalDataRequestModel.exchange = historicalDataRequestModel.exchange.ToUpper();
            historicalDataRequestModel.tradingSymbol = historicalDataRequestModel.tradingSymbol.ToUpper();

            // Getting instrument token from the trading symbol
            var instrumentToken = KiteInstruments.KiteInstrumentsCsv?
                    .AsEnumerable()
                    .Where(myRow => (myRow.Field<string>("exchange")?.ToUpper() == historicalDataRequestModel.exchange))
                    .Where(myRow => (myRow.Field<string>("tradingsymbol")?.ToUpper() == historicalDataRequestModel.tradingSymbol))
                    .Select(myRow => myRow.Field<string>("instrument_token") ?? string.Empty).FirstOrDefault();

            // Getting Historical Data
            var kiteSdk = KiteSessionHelper.GetKiteSdkFromSession(Request.Headers.Authorization);
            var result = kiteSdk?.GetHistoricalData(instrumentToken, historicalDataRequestModel.startDateTime, historicalDataRequestModel.endDateTime, historicalDataRequestModel.interval, false, true);

            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(result);
        }
    }
}
