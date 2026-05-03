using Aoun.BLL.DTOs.Auth;
using Aoun.BLL.DTOs.ChatAI;
using Aoun.BLL.DTOs.Zakat;
using Aoun.BLL.Interfaces;
using Aoun.BLL.Services.Auth;
using Aoun.BLL.Services.Chat;
using Aoun.BLL.Services.Zakat;
using Microsoft.Extensions.DependencyInjection;

namespace Aoun.API.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        //services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<AISmartService>();
        services.AddScoped<ZakatService>();
    }
}




