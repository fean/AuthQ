using System.Security.Principal;

namespace AuthQ.SSO.OAuth.Implementations
{
    public class AuthQPrincipal : OAuthPrincipalBase
    {
        public AuthQPrincipal(IOAuthProvider provider, IIdentity identity)
            : base(provider, identity)
        { }

        protected override void Load()
        {
            Roles = new[] {"User"};
        }
    }
}