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
            if (long.TryParse(filterContext.HttpContext.Request.QueryString["trustid"], out trust) && origin != null)
            {
                var T = new AuthQEntities().Trusts.FirstOrDefault(t => t.TrustId == trust)
                    .TrustedDomains.FirstOrDefault(d => d.Domain == origin);
                if (T != null)
                    filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", T.Domain);
            }
            base.OnResultExecuting(filterContext);
        }
    }
}