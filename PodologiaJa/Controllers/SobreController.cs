using Microsoft.AspNetCore.Mvc;

namespace PodologiaJa.Controllers
{
    public class SobreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
