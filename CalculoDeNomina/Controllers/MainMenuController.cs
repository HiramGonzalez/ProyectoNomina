using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CalculoDeNomina.Controllers
{
    [Authorize]
    public class MainMenuController : Controller
    {
        
        public ActionResult MainMenu()
        {
            return View("MainMenu");
        }

        public ActionResult IrAHistorialNominas()
        {
            return RedirectToAction("HistorialNominas", "HistorialNominas");
        }
    }
}