using Microsoft.AspNetCore.Mvc;

namespace Articalproject.Controllers
{
    public class AllUsers : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
