using Aoun.BLL.DTOs.Case;
using Aoun.BLL.DTOs.ChatAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aoun.BLL.Interfaces;
using Aoun.BLL.Services;

public static class BllServiceExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GeminiSettings>(configuration.GetSection("GeminiSettings"));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<IAdminService, RealAdminService>();
        services.AddScoped<ICharityService, RealCharityService>();
        services.AddScoped<IProfileService, RealProfileService>();
        services.AddScoped<AISmartService>();

        return services;
    }
}