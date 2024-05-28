using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WhiteLabel.WebAPI.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        [Obsolete("Use TimeProvider on AuthenticationSchemeOptions instead.")]
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string username = null;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                if (authHeader.Parameter != null)
                {
                    var credentials = Encoding.UTF8
                        .GetString(Convert.FromBase64String(authHeader.Parameter))
                        .Split(':');
                    username = credentials.FirstOrDefault();
                }

                // Uncomment and implement the validation logic
                // if (!await _userService.ValidateCredentials(username, password))
                //     throw new ArgumentException("Invalid credentials");
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail($"Authentication failed: {ex.Message}")
                );
            }

            if (username is null)
                return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));

            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
