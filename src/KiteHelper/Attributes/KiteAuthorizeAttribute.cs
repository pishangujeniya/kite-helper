using KiteHelper.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace KiteHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class KiteAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            StringValues? sessionId = context.HttpContext.Request?.Headers["Authorization"];

            if (sessionId.HasValue && !string.IsNullOrWhiteSpace(sessionId.GetValueOrDefault().ToString()))
            {
                if (KiteSessionHelper.IsSessionValid(sessionId))
                {
                    return Task.CompletedTask;
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                    return Task.CompletedTask;
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }
        }
    }
}
