using System;
using System.Security.Principal;
using System.Web;

namespace AuthiQ.SSO.OAuth.Implementations
{
    public class AuthiQProvider : OAuthProviderBase
    {
        public override IIdentity RetrieveIdentity(HttpContext context)
        {
            var token = context.Request.GetToken();
            return String.IsNullOrEmpty(token)
                ? null
                : new AuthiQIdentity(this, token);
        }

        public override IPrincipal CreatePrincipal(IIdentity identity)
        {
            return identity == null || !(identity is AuthiQIdentity)
                ? null
                : new AuthiQPrincipal(this, identity);
        }
    }
}