using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KiteHelper.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        private ILogger<HttpResponseExceptionFilter> _logger;

        public HttpResponseExceptionFilter(ILogger<HttpResponseExceptionFilter> logger)
        {
            _logger = logger;
        }

        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                _logger.LogError(context.Exception, $"Error occurred while executing : " +
                                                    $"\n Path : {context.HttpContext.Request.Path}" +
                                                    $"\n QueryString : {context.HttpContext.Request.QueryString}"
                );
                context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                context.ExceptionHandled = true;
            }
        }
    }
}
