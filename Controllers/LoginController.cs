using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuthQ.SSO.Attributes;
using AuthQ.SSO.Data;
using AuthQ.SSO.Models;
using AuthQ.SSO.OAuth;
using AuthQ.SSO.OAuth.Implementations;

namespace AuthQ.SSO.Controllers
{
    [NoCache]
    public class LoginController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(string requestToken, string trustid)
        {
            try
            {
                try
                {
                    if (!OAuthServiceBase.Instance.IsValidToken(requestToken) || string.IsNullOrEmpty(trustid))
                        return
                            Content(
                                "Please read the documentation on how to implement this single sign-on service. <br/>");
                }
                catch (Exception ex)
                {
                    return Content("<script type=\"text/javascript\">location.reload();</script>");
                }

                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var tokendata = Request.GetToken();
                    var token = new AuthQEntities().Tokens.FirstOrDefault(t => t.AccessToken == tokendata);

                    if (token == null)
                        return View(new LoginModel
                            {
                                RequestToken = requestToken,
                                TrustId = long.Parse(trustid),
                                Challenge = ComputeChallenge(trustid, requestToken)
                            });

                    if (token.Expire < DateTime.Now)
                    {
                        var response = OAuthServiceBase.Instance.RefreshToken(token.RefreshToken, long.Parse(trustid));
                        token = new Token()
                            {
                                AccessToken = response.AccessToken,
                                Expire = DateTime.Now.AddSeconds(response.Expires),
                                RefreshToken = response.RefreshToken
                            };
                    }

                    ViewBag.Name = token.Login.TeamMember != null
                                       ? token.Login.TeamMember.Name
                                       : token.Login.Username;

                    return View("Success", new OAuthResponse
                        {
                            AccessToken = token.AccessToken,
                            Expires = (int) token.Expire.Subtract(DateTime.Now).TotalSeconds,
                            RefreshToken = token.RefreshToken,
                            Success = true
                        });
                }


                return View(new LoginModel
                    {
                        RequestToken = requestToken,
                        TrustId = long.Parse(trustid),
                        Challenge = ComputeChallenge(trustid, requestToken)
                    });
            }
            catch (NullReferenceException ex)
            {
                return Content("Invalid trust ID, please try again with a valid one.");
            }
            catch (HttpRequestValidationException ex)
            {
                return Content("Please create a request token with your own trust ID.");
            }
            catch (Exception ex)
            {
                return Content("<script type=\"text/javascript\">location.reload();</script>");
            }
        }

        [HttpPost]
        public ActionResult Index(string requestToken, string challenge, string username, string password, bool? rememberMe, long trustid)
        {
            var accessResponse = OAuthServiceBase.Instance.AccessToken(requestToken, trustid, challenge, "User", username, password.ToHMAC(requestToken), rememberMe.HasValue && rememberMe.Value);

            if (!accessResponse.Success)
            {
                if (!OAuthServiceBase.Instance.IsValidToken(requestToken))
                    return Content("Please read the documentation on how to implement this single sign-on service. <br/>");

                return View(new LoginModel
                {
                    RequestToken = requestToken,
                    Username = username,
                    RememberMe = rememberMe.HasValue && rememberMe.Value,
                    ErrorMessage = "Invalid Username or Password, please try again.",
                    TrustId = trustid
                });
            }

            var token = new AuthQEntities().Tokens.FirstOrDefault(t => t.AccessToken == accessResponse.AccessToken);

            ViewBag.Name = token.Login.TeamMember != null
                                   ? token.Login.TeamMember.Name
                                   : token.Login.Username;

            return View("Success", accessResponse);
        }

        private string ComputeChallenge(string trust, string token)
        {
            var T = Convert.ToInt64(trust);
            var S = new AuthQEntities().Trusts.FirstOrDefault(t => t.TrustId == T).Secret;
            if (AuthQService.RequestTokens[token].Value != T)
                throw new HttpRequestValidationException();
            return token.ToHMAC(S);
        }
    }
}
