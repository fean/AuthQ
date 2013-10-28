namespace AuthQ.SSO.OAuth
{
    public static class OAuthConstants
    {
        public const string UnauthorizedHeader = "WWW-Authenticate";

        public const string AuthorzationHeader = "Authorization";

        public const string AuthorzationParam = "sso_token";

        public const int AuthorizationExpiration = 1440; /* 24-hours */
    }
}