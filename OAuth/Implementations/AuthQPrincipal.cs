using System.Security.Principal;

namespace AuthiQ.SSO.OAuth.Implementations
{
    public class AuthiQPrincipal : OAuthPrincipalBase
    {
        public AuthiQPrincipal(IOAuthProvider provider, IIdentity identity)
            : base(provider, identity)
        { }

        protected override void Load()
        {
            Roles = new[] {"User"};
        }
    }
}