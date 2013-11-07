using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuthiQ.SSO.Attributes;
using AuthiQ.SSO.Data;
using AuthiQ.SSO.Models;

namespace AuthiQ.SSO.Controllers
{
    [CheckOrigin, /*RequireHttps*/]
    public class ProfileController : Controller
    {

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string accessToken)
        {
            using (var c = new Entities())
            {
                var user = c.Tokens.FirstOrDefault(t => t.AccessToken == accessToken);
                if (user == null)
                    return Json(new Profile()
                        {
                            Success = false,
                            Error = "Access token unknown."
                        });
                return Json(new Profile()
                    {
                        ID = user.Login.Id,
                        Username = user.Login.Username,
                        MailAddress = user.Login.Mail,
                        FullName = user.Login.TeamProfile != null ? user.Login.TeamMember.Name : null
                    });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update(Profile profile, string accessToken)
        {
            return Json(new Profile()
                {
                    Success = false,
                    Error = "Profile updating is not yet implemented."
                });
        }
    }
}
