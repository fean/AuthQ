using System;
using System.Security.Principal;
using System.Web;

namespace AuthQ.SSO.OAuth.Implementations
{
    public class AuthQProvider : OAuthProviderBase
    {
        public override IIdentity RetrieveIdentity(HttpContext context)
        {
            var token = context.Request.GetToken();
            return String.IsNullOrEmpty(token)
                ? null
                : new AuthQIdentity(this, token);
        }

        public override IPrincipal CreatePrincipal(IIdentity identity)
        {
            return identity == null || !(identity is AuthQIdentity)
                ? null
                : new AuthQPrincipal(this, identity);
        }
    }
}