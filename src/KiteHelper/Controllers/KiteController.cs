using System.Data;
using System.Net;
using KiteConnect;
using KiteConnectSdk;
using KiteHelper.Attributes;
using KiteHelper.Domain.HistoricalData;
using KiteHelper.Domain.Login;
using KiteHelper.Domain.TradingSymbols;
using KiteHelper.Helpers;
using KiteHelper.Infrastructure;
using KiteHelper.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KiteHelper.Controllers
{
    [ApiController]
    [Route("api/kite")]
    public class KiteController : ControllerBase
    {
        private readonly ILogger<KiteController> _logger;
        private readonly DatabaseContext _databaseContext;

        public KiteController(ILogger<KiteController> logger, DatabaseContext databaseContext)
        {
            _logger = logger;
            _databaseContext = databaseContext;
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
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(List<KiteInstrumentsEntity>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> TradingSymbols([FromQuery()] TradingSymbolsRequestModel tradingSymbolsRequestModel)
        {
            tradingSymbolsRequestModel.tradingSymbol = tradingSymbolsRequestModel.tradingSymbol?.ToUpper() ?? string.Empty;

            var result = _databaseContext.KiteInstrumentsEntity
                .Where(myRow => string.IsNullOrWhiteSpace(tradingSymbolsRequestModel.tradingSymbol) || (myRow.TradingSymbol.ToUpper().Contains(tradingSymbolsRequestModel.tradingSymbol)))
                .OrderBy(myRow=> myRow.Expiry)
                .Select(myRow => myRow).ToList();

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
            var instrumentToken = _databaseContext.KiteInstrumentsEntity
                .Where(myRow => (myRow.Exchange.ToUpper() == historicalDataRequestModel.exchange))
                .Where(myRow => (myRow.TradingSymbol.ToUpper() == historicalDataRequestModel.tradingSymbol))
                .Select(myRow => myRow.InstrumentToken).FirstOrDefault();

            // Getting Historical Data
            var kiteSdk = KiteSessionHelper.GetKiteSdkFromSession(Request.Headers.Authorization);
            var result = kiteSdk?.GetHistoricalData(instrumentToken, historicalDataRequestModel.startDateTime, historicalDataRequestModel.endDateTime, historicalDataRequestModel.interval, false, true);

            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(result);
        }
    }
}
