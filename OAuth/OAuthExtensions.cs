using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AuthiQ.SSO.OAuth
{
    public static class OAuthExtensions
    {
        public static string GetToken(this HttpRequest request)
        {
            var wrapper = new HttpRequestWrapper(request);
            return GetToken(wrapper);
        }

        public static string GetRawContent(this HttpRequest request)
        {
            request.InputStream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(request.InputStream).ReadToEnd();
        }

        public static string GetToken(this HttpRequestBase request)
        {
            if (request == null)
                return String.Empty;

            // Find Header
            var headerText = request.Headers[OAuthConstants.AuthorzationHeader];
            if (!String.IsNullOrEmpty(headerText))
            {
                var header = new AuthorizationHeader(headerText);
                if (string.Equals(header.Scheme, "OAuth", StringComparison.OrdinalIgnoreCase))
                    return header.ParameterText.Trim();
            }

            // Find Clean Param
            var token = request.Params[OAuthConstants.AuthorzationParam];
            return !String.IsNullOrEmpty(token)
                ? token.Trim()
                : String.Empty;
        }

        public static string ToSHA(this string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            var hasher = new SHA512Managed();
            var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            return BitConverter.ToString(data).Replace("-", String.Empty).ToLower();
        }

        public static string ToHMAC(this string input, string key)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            var hasher = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            var retval = BitConverter.ToString(data).Replace("-", String.Empty).ToLower();

            return retval;
        }
    }
}