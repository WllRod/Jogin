using Microsoft.AspNetCore.Mvc;

namespace Jogin.Controllers
{
    public class JogoController : Controller
    {
        public IActionResult Index()
        {
            if(String.IsNullOrEmpty(HttpContext.Session.GetString("username")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
