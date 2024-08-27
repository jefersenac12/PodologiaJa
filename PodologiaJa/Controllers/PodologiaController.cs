using Microsoft.AspNetCore.Mvc;

namespace PodologiaJa.Controllers
{
    public class PodologiaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
