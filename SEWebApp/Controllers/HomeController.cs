using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEConnect.Business;
namespace TaskWebApp.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SEConnect.Business.EntityProcessors.ConnectorEntities connectors = new SEConnect.Business.EntityProcessors.ConnectorEntities();

            var result = connectors.GetConnectors();

            return View();
        }

        public ActionResult LoginPage()
        {
            return View();
        }


        public ActionResult SignupPage()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Error(string message)
        {
            ViewBag.Message = message;

            return View("Error");
        }
    }
}