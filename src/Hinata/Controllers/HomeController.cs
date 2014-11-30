using System.Web.Mvc;

namespace Hinata.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View(!Request.IsAuthenticated ? "Login" : "Index");
        }
    }
}