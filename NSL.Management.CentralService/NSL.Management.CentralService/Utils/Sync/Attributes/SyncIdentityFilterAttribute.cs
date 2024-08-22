using Microsoft.AspNetCore.Mvc.Filters;
using NSL.Management.CentralService.Utils.Sync.Services;

namespace NSL.Management.CentralService.Utils.Sync.Attributes
{
    public class SyncIdentityFilterAttribute : ActionFilterAttribute
    {
        static SyncIdentityService IdentityService { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IdentityService ??= context.HttpContext.RequestServices.GetRequiredService<SyncIdentityService>();

            if (!await IdentityService.TrySignAsync(context, context.HttpContext.Request.Headers))
            {
                context.HttpContext.Response.StatusCode = 401;
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
