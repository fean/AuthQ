using System.Linq;
using System.Web;

namespace AuthQ.SSO.Json
{
    public static class HttpRequestExtensions
    {
        public static bool IsJsonpRequest(this HttpRequestBase request)
        {
            return request.Params.AllKeys.Contains(JsonpResult.JsonCallbackKey);
        }
    }
}