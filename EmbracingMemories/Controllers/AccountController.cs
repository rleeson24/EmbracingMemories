using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace EmbracingMemories.Controllers
{
	[CustomErrorHandler]
	[RequireHttps]
	public class AccountController : Controller
	{
		private ApplicationUserManager _userManager;

		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//
		// GET: /Account/Login
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}


		//
		// GET: /Account/Register
		public ActionResult Register()
		{
			return View();
		}


		//GET: /Account/ConfirmEmail
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			ViewBag.TokensMissing = false;
			ViewBag.HasPassword = false;
			if (userId == null || code == null)
			{
				ViewBag.TokensMissing = true;
				return View();
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);
			String token = null;
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(userId);
				ViewBag.HasPassword = UserManager.HasPassword(user.Id);
				ViewBag.Message = user.Email + " has been Confirmed!";
				ViewBag.EmailAddress = user.Email;
				token = await UserManager.GenerateUserTokenAsync("SetPassword",userId);
				ViewBag.Code = token;
			}
			else
			{
				ViewBag.ErrorMessage = "Could not confirm your registration info.";
				ViewBag.ResendEmailConfirmationTokenLink = Url.Action("ResendEmailConfirmationToken", new { controller = "Account", area = "Account", userId = userId });
			}
			return View();
		}

		//
		// GET: /Account/ForgotPassword
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// GET: /Account/SetPassword
		public ActionResult SetPassword(String emailAddress, String code)
		{
			ViewBag.Code = code;
			ViewBag.EmailAddress = emailAddress;
			return code == null ? View("Error") : View();
		}


		//
		// GET: /Account/SetPasswordConfirmation
		public ActionResult SetPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ChangePassword
		[Authorize]
		public ActionResult ChangePassword()
		{
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ExternalLoginFailure
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}