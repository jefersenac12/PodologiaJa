using Microsoft.AspNetCore.Mvc;

namespace PodologiaJa.Controllers
{
    public class ServicosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
