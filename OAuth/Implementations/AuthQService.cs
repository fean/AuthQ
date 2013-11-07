using System;
using System.Collections.Generic;
using System.Linq;
using AuthiQ.SSO.Data;

namespace AuthiQ.SSO.OAuth.Implementations
{
    public class AuthiQService : OAuthServiceBase
    {
        static AuthiQService()
        {
            RequestTokens = new Dictionary<string, KeyValuePair<DateTime, long>>();
        }

        public static Dictionary<string, KeyValuePair<DateTime, long>> RequestTokens { get; set; }

        public override OAuthResponse RequestToken(string trustId)
        {
            long trustlong;
            if (String.IsNullOrEmpty(trustId) || !long.TryParse(trustId, out trustlong))
                return new OAuthResponse
                    {
                        Success = false,
                        Error = "Invalid Trust ID."
                    };

            var trust = new Entities().Trusts.FirstOrDefault(t => t.TrustId == trustlong);

            if (trust == null)
                return new OAuthResponse
                    {
                        Success = false,
                        Error = "Invalid Trust ID."
                    };

            var token = Guid.NewGuid().ToString("N");
            var expire = DateTime.Now.AddMinutes(5);
            RequestTokens.Add(token, new KeyValuePair<DateTime, long>(expire, trustlong));

            return new OAuthResponse
                   {
                       Expires = (int)expire.Subtract(DateTime.Now).TotalSeconds,
                       RequestToken = token,
                       RequireSsl = false,
                       Success = true
                   };
        }

        public override OAuthResponse AccessToken(string requestToken, long trustid, string challenge, string grantType, string userName, string password, bool persistent)
        {
            if (!RequestTokens.Keys.Contains(requestToken))
                return new OAuthResponse
                {
                    Error = "Request token unknown.",
                    Success = false
                };
            var tokenData = RequestTokens[requestToken];
            if (DateTime.Now > tokenData.Key)
            {
                RequestTokens.Remove(requestToken);
                return new OAuthResponse
                {
                    Error = "Request token expired.",
                    Success = false
                };
            }

            if (tokenData.Value != trustid)
                return new OAuthResponse
                {
                    Error = "Request token not requested by this trust.",
                    Success = false
                };

            var context = new Entities();
            var trust = context.Trusts.FirstOrDefault(t => t.TrustId == tokenData.Value);

            if (requestToken.ToHMAC(trust.Secret) != challenge)
                return new OAuthResponse
                {
                    Success = false,
                    Error = "Invalid challenge for this request."
                };

            trust = null;
            var login = context.Logins.FirstOrDefault(u => u.Username == userName);

            if (login == null)
                return new OAuthResponse
                {
                    Success = false,
                    Error = "Invalid username or password."
                };

            var hash = login.Password.ToHMAC(requestToken);

            if (!password.Equals(hash))
                return new OAuthResponse
                       {
                           Success = false
                       };

            RequestTokens.Remove(requestToken);
            return CreateAccessToken(login, context, trustid);
        }

        public override OAuthResponse RefreshToken(string refreshToken, long trustid)
        {
            using (var c = new Entities())
            {
                var token = c.Tokens.FirstOrDefault(t => t.RefreshToken == refreshToken);

                if (token == null)
                    return new OAuthResponse
                    {
                        Error = "RefreshToken not found.",
                        Success = false
                    };
                if (token.Trust1.TrustId != trustid)
                    return new OAuthResponse
                    {
                        Error = "Invalid trust id.",
                        Success = false
                    };

                var response = CreateAccessToken(token.Login, c, trustid);
                
                c.Tokens.Remove(token);
                c.SaveChanges();

                return response;
            }
        }

        private OAuthResponse CreateAccessToken(Login user, Entities context, long trust)
        {
            var trust_ = context.Trusts.FirstOrDefault(t => t.TrustId == trust);
            var token = new Token((Login)user, trust_);

            context.Tokens.Add(token);
            context.SaveChanges();

            return new OAuthResponse
            {
                AccessToken = token.AccessToken,
                Expires = (int)token.Expire.Subtract(DateTime.Now).TotalSeconds,
                RefreshToken = token.RefreshToken,
                Trust = trust,
                RequireSsl = false,
                Success = true
            };
        }

        public override OAuthResponse CheckToken(string type, string token)
        {
            if (type.ToLower() == "request")
            {
                if (!RequestTokens.Keys.Contains(token))
                    return new OAuthResponse
                    {
                        Error = "Request token unknown.",
                        Success = false
                    };
                var rToken = RequestTokens[token];
                return new OAuthResponse
                    {
                        Success = true,
                        RequestToken = token,
                        Expires = (int)rToken.Key.Subtract(DateTime.Now).TotalSeconds,
                        Trust = rToken.Value
                    };
            }
            if (type.ToLower() == "auth")
            {
                var aToken = new Entities().Tokens.FirstOrDefault(t => t.AccessToken == token);
                if (aToken == null)
                    return new OAuthResponse
                    {
                        Error = "Authentication token unknown.",
                        Success = false
                    };
                return new OAuthResponse
                    {
                        Success = true,
                        AccessToken = aToken.AccessToken,
                        Expires = (int)aToken.Expire.Subtract(DateTime.Now).TotalSeconds,
                        RefreshToken = aToken.RefreshToken,
                        Trust = aToken.Trust1.TrustId
                    };
            }
            return new OAuthResponse
            {
                Error = "Token type unknown.",
                Success = false
            };
        }

        public override bool UnauthorizeToken(string accessToken)
        {
            using (var c = new Entities())
            {
                var token = c.Tokens.FirstOrDefault(t => t.AccessToken == accessToken);

                if (token == null)
                    return false;

                c.Tokens.Remove(token);
                c.SaveChanges();

                return true;
            }
        }

        public override bool IsValidToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            if (!RequestTokens.Keys.Contains(token))
                return false;

            if (DateTime.Now > RequestTokens[token].Key)
            {
                RequestTokens.Remove(token);
                return false;
            }
            return true;
        }
    }
}