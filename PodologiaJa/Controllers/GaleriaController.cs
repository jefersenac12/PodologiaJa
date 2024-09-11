using Microsoft.AspNetCore.Mvc;

namespace PodologiaJa.Controllers
{
    public class GaleriaController : Controller
    {
        public IActionResult GaleriaAnteseDepois()
        {
            return View();
        }
    }
}
