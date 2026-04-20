using Microsoft.Extensions.DependencyInjection;
using shared.Utilities;

namespace shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContextClaimPrincipal>();
        return services;
    }
}