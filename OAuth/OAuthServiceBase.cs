using System.Collections.Specialized;
using System.Configuration.Provider;

namespace AuthiQ.SSO.OAuth
{
    public interface IOAuthService
    {
        OAuthResponse RequestToken(string trustId);
        OAuthResponse AccessToken(string requestToken, long trustid, string challenge, string grantType, string userName, string password, bool persistent);
        OAuthResponse RefreshToken(string refreshToken, long trustid);
        OAuthResponse CheckToken(string type, string token);
        bool UnauthorizeToken(string token);
        bool IsValidToken(string token);

        void Initialize(string name, NameValueCollection config);
        string Name { get; }
        string Description { get; }
    }

    public abstract class OAuthServiceBase : ProviderBase, IOAuthService
    {
        public static IOAuthService Instance { get; set; }

        public abstract OAuthResponse RequestToken(string trustId);

        public abstract OAuthResponse AccessToken(string requestToken, long trustid, string challenge, string grantType, string userName, string password, bool persistent);

        public abstract OAuthResponse RefreshToken(string refreshToken, long trustid);

        public abstract OAuthResponse CheckToken(string type, string token);

        public abstract bool UnauthorizeToken(string token);

        public abstract bool IsValidToken(string token);
    }
}