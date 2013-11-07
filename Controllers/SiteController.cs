using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuthiQ.SSO.Controllers
{
    [RequireHttps]
    public class SiteController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

    }
}
