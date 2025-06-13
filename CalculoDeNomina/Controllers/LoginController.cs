using CalculoDeNomina.Models.Repositorios;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcMovie.Controllers
{
    public class LoginController : Controller
    {
        private UsuarioRepositorio _usuarioRepo = new UsuarioRepositorio();

        [AllowAnonymous]
        public ActionResult CargarPagina()
        {
            ViewBag.MostrarMensajesError = false;
            return View("Login");
        }

        // Método Login
        [HttpPost]
        public ActionResult LoginUsuario(string nombreUsuario, string password)
        {
            if (_usuarioRepo.Login(nombreUsuario, password))
            {
                FormsAuthentication.SetAuthCookie(nombreUsuario, false);
                return RedirectToAction("MainMenu", "MainMenu");
            }
            else
            {
                ViewBag.MostrarMensajesError = true;
                return View("Login");
            }

        }

        // Método para salir de la cuenta
        public ActionResult LogoutUsuario()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("CargarPagina", "Login");
        }

    }
}