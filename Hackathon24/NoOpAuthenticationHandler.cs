using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Hackathon24
{

    public class NoOpAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public NoOpAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Create a no-op authentication ticket (no real user is authenticated)
            var claims = new[] { new Claim(ClaimTypes.Name, "DemoUser") };
            var identity = new ClaimsIdentity(claims, "NoAuthScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "NoAuthScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

}
