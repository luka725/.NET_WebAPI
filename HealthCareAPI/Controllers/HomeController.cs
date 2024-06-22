using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthCareAPI.Controllers
{
    /// <summary>
    /// Controller for handling home-related actions.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// GET: Home/Index
        /// Action method for rendering the home page.
        /// </summary>
        /// <returns>The Index view result.</returns>
        public ActionResult Index()
        {
            // Set the title of the view
            ViewBag.Title = "Home Page";

            // Return the Index view
            return View();
        }
    }
}
