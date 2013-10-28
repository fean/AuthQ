using System.Linq;
using System.Web.Mvc;
using AuthQ.SSO.Data;

namespace AuthQ.SSO.Attributes
{
    public class CheckOriginAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            long trust;
            var origin = filterContext.HttpContext.Request.Headers["Origin"];
            if (origin != null)
            {
                if (long.TryParse(filterContext.HttpContext.Request.QueryString["trustid"], out trust))
                {
                    var T = new AuthQEntities().Trusts.FirstOrDefault(t => t.TrustId == trust)
                                               .TrustedDomains.FirstOrDefault(d => d.Domain == origin);
                    if (T != null)
                        filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", T.Domain);
                }
                else if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["accessToken"]) || string.IsNullOrEmpty(filterContext.HttpContext.Request.Form["accessToken"]))
                {
                    var token = string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["accessToken"])
                                                 ? filterContext.HttpContext.Request.Form["accessToken"]
                                                 : filterContext.HttpContext.Request.QueryString["accessToken"];

                    var T = new AuthQEntities().Tokens.FirstOrDefault(t => t.AccessToken == token)
                            .Trust1.TrustedDomains.FirstOrDefault(d => d.Domain == origin);
                    if (T != null)
                        filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", T.Domain);
                }
            }
            base.OnResultExecuting(filterContext);
        }
    }
}