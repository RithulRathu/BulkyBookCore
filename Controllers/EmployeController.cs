using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Controllers
{
    public class EmployeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
