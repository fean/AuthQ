using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuthiQ.SSO.Views
{
    public class DeveloperController : Controller
    {

        public ActionResult Demonstration(string type)
        {
            if(type == "js")
                return View("JSDemonstration");
            return View("Error");
        }

    }
}
