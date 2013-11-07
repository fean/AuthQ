using System;
using System.Linq;
using AuthiQ.SSO.Data;

namespace AuthiQ.SSO.OAuth.Implementations
{
    public class AuthiQIdentity : OAuthIdentityBase
    {
        public AuthiQIdentity(IOAuthProvider provider, string token)
            : base(provider)
        {
            Token = token;
            Realm = "AuthiQ";
        }

        protected override void Load()
        {
            var token = new Entities().Tokens.FirstOrDefault(t =>  DateTime.Now > t.Expire && t.AccessToken == Token);
            if (token == null)
                return;

            IsAuthenticated = true;
            Name = token.Login.Username;
        }
    }
}