using Aoun.BLL.Services;

namespace Aoun.API.Middleware
{
    public class CampaignStateMiddleware
    {
        private readonly RequestDelegate _next;

        public CampaignStateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, CampaignStateService campaignStateService)
        {
            await campaignStateService.SyncCampaignStatesAsync();

            await _next(context);
        }
    }
}