using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Petsmart.Controllers
{
    public class AProductsController : Controller
    {
        // GET: AProducts
        public ActionResult Index()
        {
            return View();
        }
    }
}