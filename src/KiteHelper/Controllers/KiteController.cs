using System.Data;
using System.Net;
using System.Text;
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
                        return new JsonResult(new KiteLoginResponseModel() { SessionId = sessionId });
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
            catch (WebException we)
            {
                var (statusCode, responseString) = we.GetResponseStringNoException();
                Response.StatusCode = (int)statusCode;
                return Content(responseString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in {nameof(Login)}");
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
            try
            {
                var kiteSdk = KiteSessionHelper.GetKiteSdkFromSession(Request.Headers.Authorization);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return new JsonResult(kiteSdk?.GetProfile());
            }
            catch (WebException we)
            {
                var (statusCode, responseString) = we.GetResponseStringNoException();
                Response.StatusCode = (int)statusCode;
                return Content(responseString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in {nameof(Profile)}");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("trading-symbols")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(List<KiteInstrumentsEntity>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> TradingSymbols([FromQuery()] TradingSymbolsRequestModel tradingSymbolsRequestModel)
        {
            try
            {
                tradingSymbolsRequestModel.TradingSymbol = tradingSymbolsRequestModel.TradingSymbol?.ToUpper() ?? string.Empty;

                var result = _databaseContext.KiteInstrumentsEntity
                    .Where(myRow => string.IsNullOrWhiteSpace(tradingSymbolsRequestModel.TradingSymbol) || (myRow.TradingSymbol.ToUpper().Contains(tradingSymbolsRequestModel.TradingSymbol)))
                    .OrderBy(myRow => myRow.Expiry)
                    .Select(myRow => myRow).ToList();

                Response.StatusCode = (int)HttpStatusCode.OK;
                return new JsonResult(result);
            }
            catch (WebException we)
            {
                var (statusCode, responseString) = we.GetResponseStringNoException();
                Response.StatusCode = (int)statusCode;
                return Content(responseString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in {nameof(TradingSymbols)}");
                return new StatusCodeResult(500);
            }
        }

        [HttpPost]
        [Route("historical-data")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(List<Historical>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [KiteAuthorize]
        public async Task<ActionResult> HistoricalData([FromBody] HistoricalDataRequestModel historicalDataRequestModel)
        {
            try
            {
                historicalDataRequestModel.Exchange = historicalDataRequestModel.Exchange.ToUpper();
                historicalDataRequestModel.TradingSymbol = historicalDataRequestModel.TradingSymbol.ToUpper();

                // Getting instrument token from the trading symbol
                var instrumentToken = _databaseContext.KiteInstrumentsEntity
                    .Where(myRow => (myRow.Exchange.ToUpper() == historicalDataRequestModel.Exchange))
                    .Where(myRow => (myRow.TradingSymbol.ToUpper() == historicalDataRequestModel.TradingSymbol))
                    .Select(myRow => myRow.InstrumentToken).FirstOrDefault();

                // Getting Historical Data
                var kiteSdk = KiteSessionHelper.GetKiteSdkFromSession(Request.Headers.Authorization);

                List<Historical> result = new List<Historical>();
                foreach (var dateRange in SplitDateRange(historicalDataRequestModel.StartDateTime, historicalDataRequestModel.EndDateTime, 50))
                {
                    var chunk = kiteSdk?.GetHistoricalData(instrumentToken, dateRange.Item1.ToLocalTime(), dateRange.Item2.ToLocalTime(), historicalDataRequestModel.Interval, false, true);
                    if (chunk != null)
                    {
                        result.AddRange(chunk);
                    }
                }

                Response.StatusCode = (int)HttpStatusCode.OK;
                return new JsonResult(result);


                IEnumerable<Tuple<DateTime, DateTime>> SplitDateRange(DateTime start, DateTime end, int dayChunkSize)
                {
                    DateTime chunkEnd;
                    while ((chunkEnd = start.AddDays(dayChunkSize)) < end)
                    {
                        yield return Tuple.Create(start, chunkEnd);
                        start = chunkEnd;
                    }
                    yield return Tuple.Create(start, end);
                }
            }
            catch (WebException we)
            {
                var (statusCode, responseString) = we.GetResponseStringNoException();
                Response.StatusCode = (int)statusCode;
                return Content(responseString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in {nameof(HistoricalData)}");
                return new StatusCodeResult(500);
            }
        }
    }
}
