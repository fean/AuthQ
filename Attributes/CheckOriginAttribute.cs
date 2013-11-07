using System;
using System.Linq;
using System.Web.Mvc;
using AuthiQ.SSO.Data;

namespace AuthiQ.SSO.Attributes
{
    public class CheckOriginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            long trust;
            var origin = filterContext.HttpContext.Request.Headers["Origin"];
            if (origin != null)
            {
                if (long.TryParse(filterContext.HttpContext.Request.QueryString["trustid"], out trust))
                {
                    var T = new Entities().Trusts.FirstOrDefault(t => t.TrustId == trust)
                                               .TrustedDomains.FirstOrDefault(d => d.Domain == origin);
                    if (T != null)
                        filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", T.Domain);
                }
                else if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["accessToken"]) || !string.IsNullOrEmpty(filterContext.HttpContext.Request.Form["accessToken"]))
                {
                    try
                    {
                        var token = string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["accessToken"])
                                        ? filterContext.HttpContext.Request.Form["accessToken"]
                                        : filterContext.HttpContext.Request.QueryString["accessToken"];
                        using (var c = new Entities())
                        {
                            var T = c.Tokens.FirstOrDefault(t => t.AccessToken == token)
                            .Trust1.TrustedDomains.FirstOrDefault(d => d.Domain == origin);

                            if (T != null)
                                filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", T.Domain);
                        }
                    } catch(NullReferenceException ex) {}
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}