﻿using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SocialApp.TokenValidator
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly FirebaseApp _firebaseApp;
        public FirebaseAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, FirebaseApp firebaseApp) : base(options, logger, encoder, clock)
        {
            _firebaseApp = firebaseApp;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request. Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }
            try
            {
                string bearerToken = Context.Request.Headers ["Authorization"];

                if (bearerToken == null || !bearerToken.StartsWith("Bearer "))
                {
                    return AuthenticateResult.Fail("Invalid scheme.");
                }

                string token = bearerToken.Substring("Bearer ".Length);

                var firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);

                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new List<ClaimsIdentity>()
                {
                    new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(FirebaseAuthenticationHandler))
                }), JwtBearerDefaults.AuthenticationScheme));

            } catch (FirebaseAuthException ex)
            {
                return AuthenticateResult.Fail(ex);
            }

        }
        private IEnumerable<Claim>ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            return new List<Claim> {
                new Claim("id", claims["user_id"].ToString()),
                new Claim ("email", claims["email"].ToString()),
                new Claim("time", claims["auth_time"].ToString()),
            };
        }
    }
}



