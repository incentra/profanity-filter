
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SP.Profanity.Interfaces;

namespace SP.Profanity.Handlers
{
    
    public class ApiDashboardAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAppSettings _appSettings;
        private readonly string authKey = "Authorization";

        public ApiDashboardAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAppSettings appSettings) : base(options, logger, encoder, clock)
        {
            _appSettings = appSettings;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticateResult authenticateResult;
            try{
                Endpoint endpoint = Context.GetEndpoint();
                if(endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null) authenticateResult = await Task.FromResult(AuthenticateResult.NoResult());
                else if (!Request.Headers.ContainsKey(authKey)) authenticateResult = await Task.FromResult(AuthenticateResult.Fail($"Missing Authorization Header."));
                else if(Request.Headers[authKey] != _appSettings.ApiToken) authenticateResult = await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header."));
                else authenticateResult = await InitFilterDashboardSuccess();
            }
            catch(Exception ex)
            {
                authenticateResult = await Task.FromResult(AuthenticateResult.Fail(ex));
                this.Logger?.LogError("SP.Profanity.ApiDashboardAuthHandler.HandleAuthenticateAsync", "Auth fail with access to {0}. Error: {1}", Request.Path, ex.Message);
            }
            
            return authenticateResult;
        }

        private async Task<AuthenticateResult> InitFilterDashboardSuccess()
        {
            Claim[] claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "filter-dashboard"),
                new Claim(ClaimTypes.Name, "FilterDashboard"),
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}