using System.Web.Mvc;

namespace EmbracingMemories.Controllers
{
	public class EmailController : Controller
	{
		public ActionResult Registration()
		{
			return View();
		}

		public ActionResult Error()
		{
			return View();
		}
	}
}