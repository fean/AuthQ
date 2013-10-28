using System;
using System.Linq;
using AuthQ.SSO.Data;

namespace AuthQ.SSO.OAuth.Implementations
{
    public class AuthQIdentity : OAuthIdentityBase
    {
        public AuthQIdentity(IOAuthProvider provider, string token)
            : base(provider)
        {
            Token = token;
            Realm = "AuthQ";
        }

        protected override void Load()
        {
            var token = new AuthQEntities().Tokens.FirstOrDefault(t =>  DateTime.Now > t.Expire && t.AccessToken == Token);
            if (token == null)
                return;

            IsAuthenticated = true;
            Name = token.Login.Username;
        }
    }
}