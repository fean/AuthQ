using System;
using System.Web.Mvc;
using AuthiQ.SSO.Attributes;
using AuthiQ.SSO.Json;
using AuthiQ.SSO.Models;
using AuthiQ.SSO.OAuth;

namespace AuthiQ.SSO.Controllers
{
    [NoCache, CheckOrigin, RequireHttps]
    public class OAuthController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult RequestToken(string trustid)
        {
            var response = OAuthServiceBase.Instance.RequestToken(trustid);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AccessToken(string grant_type, string username, string password, bool? persistent, string request_token, string challenge, long trustid)
        {
            var response = OAuthServiceBase.Instance.AccessToken(request_token, trustid, challenge, grant_type, username, password, persistent.HasValue && persistent.Value);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult RefreshToken(string refreshToken, string trustid)
        {
            if (String.IsNullOrEmpty(refreshToken))
                return Json(new OAuthResponse
                    {
                        Success = false,
                        Error = "No refresh token supplied."
                    }, JsonRequestBehavior.AllowGet);

            long trust;
            if (!long.TryParse(trustid, out trust))
                return Json(new OAuthResponse
                {
                    Success = false,
                    Error = "Invalid trust id."
                }, JsonRequestBehavior.AllowGet);

            var response = OAuthServiceBase.Instance.RefreshToken(refreshToken, trust);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Unauthorize(string accesstoken)
        {
            var response = new JsonResponse();

            var accessToken = string.IsNullOrEmpty(accesstoken) ? Request.GetToken() : accesstoken;
            response.Success = OAuthServiceBase.Instance.UnauthorizeToken(accessToken);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CheckToken(string type, string token)
        {
            var response = new OAuthResponse();

            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(token))
            {
                response = OAuthServiceBase.Instance.CheckToken(type, token);
            }
            else
            {
                response.Success = false;
                response.Error = "The parameters Token and Type are both required.";
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return Request.IsJsonpRequest()
                ? new JsonpResult(data, contentType, contentEncoding, behavior)
                : base.Json(data, contentType, contentEncoding, behavior);
        }
    }
}
