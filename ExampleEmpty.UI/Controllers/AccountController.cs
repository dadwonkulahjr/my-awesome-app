using Microsoft.AspNetCore.Mvc;

namespace ExampleEmpty.UI.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
