using EmbracingMemories.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace EmbracingMemories.Controllers
{
	[CustomErrorHandler]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Title = "About";

			return View();
		}

		public ActionResult Archive()
		{
			ViewBag.Title = "Archive";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Title = "Contact";

			return View();
		}

		[Authorize]
		public ActionResult QrProfile()
		{
			ViewBag.Title = "Profile";

			return View();
		}

		public ActionResult ViewProfile(Guid id)
		{
			ViewBag.Title = "Profile";
			var profile = new QrContext().QrProfiles.FirstOrDefault(p => p.QrCodeGuid == id);
			if (profile != null)
			{
				return View(profile);
			}
			else
			{
				if (User.IsInRole("Admin"))
				{
					return new RedirectResult(String.Format("~/BindQrCode/{0}", id.ToString()));
				}
				else
				{
					return new RedirectResult(String.Format("~/Profiles/Create/{0}", id.ToString()));
				}
			}
		}

		public ActionResult BindQrCode(Guid id)
		{
			ViewBag.QrGuid = id.ToString();
			var unassignedProfiles = new QrContext().QrProfiles.Where(p => p.QrCodeGuid == Guid.Empty).Select(p => new { FirstName = p.FirstName, MiddleName = p.MiddleName, LastName = p.LastName, Id = p.Id });
			return View(unassignedProfiles);
		}
	}
}
