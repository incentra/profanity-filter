
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using SP.Profanity.Handlers;

namespace SP.Profanity.Helpers
{

    public class ApiDashboardAuthSchemeOptions: AuthenticationSchemeOptions
    {
        public const string Name = "ApiDashboardAuthScheme";
    }

    public static class ApiDashboardAuthExtensions
    {
        public static void UseApiDashboardAuth(this IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.DefaultScheme = ApiDashboardAuthSchemeOptions.Name;
            }).AddScheme<AuthenticationSchemeOptions, ApiDashboardAuthHandler>(ApiDashboardAuthSchemeOptions.Name, option => {});
        }
    }

}