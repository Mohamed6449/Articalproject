using Microsoft.AspNetCore.Mvc;

namespace Articalproject.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
