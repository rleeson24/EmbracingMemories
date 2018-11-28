using EmbracingMemories.Areas.Account.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace EmbracingMemories.Areas.QrProfiles.Controllers
{
	[CustomErrorHandler]
	public class ProfileViewsController : Controller
	{

		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		// GET: QrProfiles/QrProfileViews
		public PartialViewResult ReadOnlyView()
		{
			return PartialView();
		}

		public PartialViewResult Create()
		{
			if (!User.IsInRole("Admin") && !User.IsInRole("BusinessUser"))
			{
				var userDetail = UserManager.FindById(User.Identity.GetUserId());
				return PartialView(userDetail);
			}
			else
			{
				return PartialView(new ApplicationUser());
			}
		}

		public PartialViewResult List()
		{
			return PartialView();
		}

		public PartialViewResult Edit()
		{
			return PartialView();
		}
	}
}