using Microsoft.AspNetCore.Mvc;

namespace webOnpeMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

