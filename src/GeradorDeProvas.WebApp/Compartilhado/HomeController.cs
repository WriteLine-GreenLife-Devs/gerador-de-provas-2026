using Microsoft.AspNetCore.Mvc;

namespace GeradorDeProvas.WebApp.Compartilhado;

public class HomeController : Controller
{
    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }
}
