using EmbracingMemories.Areas.Account.Models;
using EmbracingMemories.Areas.QrProfiles.Models;
using EmbracingMemories.Models;
using EmbracingMemories.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace EmbracingMemories.Areas.QrProfiles.Controllers
{
	[CustomErrorHandler]
	[RoutePrefix("api/QrProfiles")]
	public class QrProfilesController : ApiController
	{
		private QrContext db = new QrContext();

		private Boolean IsProd
		{
			get
			{
				return !HttpContext.Current.Request.Url.Host.Contains("localhost");
			}
		}

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

		// GET: api/QrProfile
		[HttpGet]
		[Authorize]
		public IEnumerable<QrProfile> GetQrProfiles()
		{
			var userId = this.User.Identity.GetUserId();
			var profileIds = db.UserToProfileRelationships.Where(p => p.UserId == userId).Select(r => r.ProfileId).ToArray();
			var profiles = db.QrProfiles.Where(p => profileIds.Contains(p.Id)).ToArray();
			return profiles;
		}

		// GET: api/QrProfile/5
		[ResponseType(typeof(QrProfile))]
		[HttpGet]
		[Authorize]
		public async Task<IHttpActionResult> GetQrProfile(Guid id)
		{
			var userId = this.User.Identity.GetUserId();
			var profiles = db.UserToProfileRelationships.Where(p => p.UserId == userId).Select(r => r.ProfileId).ToArray();
			var qrProfile = await db.QrProfiles.FindAsync(id).ContinueWith(t =>
			{
				return (t.Result != null && profiles.Contains(t.Result.Id)) ? t.Result : null;
			});
			if (qrProfile != null)
			{
				if (User.IsInRole("Admin")
					|| (User.IsInRole("CanCreateProfiles") && qrProfile.CreatedByUserId == User.Identity.GetUserId())
					|| (User.IsInRole("BasicUsers") && qrProfile.UserId == User.Identity.GetUserId()))
				{
					var relationships = db.UserToProfileRelationships.Where(p => p.ProfileId == qrProfile.Id).ToArray();
					qrProfile.Users = relationships.Select(p => UserManager.GetEmail(p.UserId));
				}
				qrProfile.Photos = qrProfile.Photos.OrderBy(p => p.UploadedOn).ToArray();
				qrProfile.AudioFiles = qrProfile.AudioFiles.OrderBy(p => p.UploadedOn).ToArray();
				qrProfile.Videos = qrProfile.Videos.OrderBy(p => p.UploadedOn).ToArray();
			}
			else
			{
				return NotFound();
			}

			return Ok(qrProfile);
		}

		// GET: api/QrProfile/5
		[ResponseType(typeof(QrProfile))]
		[HttpGet]
		[Route("RetrieveForView")]
		public async Task<IHttpActionResult> RetrieveProfile(int id)
		{
			QrProfile QrProfile = await db.QrProfiles.FindAsync(id);
			if (QrProfile == null)
			{
				return NotFound();
			}

			return Ok(QrProfile);
		}

		// PUT: api/QrProfile/5
		[ResponseType(typeof(QrProfile))]
		[HttpPut]
		[Authorize(Roles = "BasicUser,Admin")]
		public async Task<IHttpActionResult> PutQrProfile(Guid id, QrProfile profile)
		{
			var existingParent = db.QrProfiles
			   .Where(p => p.Id == profile.Id)
			   .Include(p => p.Links)
			   .SingleOrDefault();

			if (existingParent != null)
			{
				foreach (var link in profile.Links)
				{
					link.QrProfileId = profile.Id;
				}
				var userId = this.User.Identity.GetUserId();
				profile.UserId = userId;
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				if (id != profile.Id)
				{
					return BadRequest();
				}

				// Update parent
				db.Entry(existingParent).CurrentValues.SetValues(profile);

				#region Links
				// Delete children
				foreach (var existingChild in existingParent.Links.ToList())
				{
					if (!profile.Links.Any(c => c.Id == existingChild.Id))
						db.Links.Remove(existingChild);
				}

				// Update and Insert children
				foreach (var childModel in profile.Links)
				{
					var existingChild = existingParent.Links
						.Where(c => c.Id == childModel.Id)
						.SingleOrDefault();

					if (existingChild != null)
						// Update child
						db.Entry(existingChild).CurrentValues.SetValues(childModel);
					else
					{
						// Insert child
						var newChild = new QrLink
						{
							Id = childModel.Id,
							Label = childModel.Label,
							QrProfileId = childModel.QrProfileId,
							Url = childModel.Url
						};
						existingParent.Links.Add(newChild);
					}
				}
				#endregion

				try
				{
					await db.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!QrProfileExists(id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
			}
			//var i = 0;
			//foreach (var l in QrProfile.Links)
			//{
			//	l.QrProfileId = QrProfile.Id;
			//	l.Id = i++;
			//}
			//var userId = this.User.Identity.GetUserId();
			//QrProfile.UserId = userId;
			//if (!ModelState.IsValid)
			//{
			//	return BadRequest(ModelState);
			//}

			//if (id != QrProfile.Id)
			//{
			//	return BadRequest();
			//}

			//db.Entry(QrProfile).State = EntityState.Modified;

			//try
			//{
			//	await db.SaveChangesAsync();
			//}
			//catch (DbUpdateConcurrencyException)
			//{
			//	if (!QrProfileExists(id))
			//	{
			//		return NotFound();
			//	}
			//	else
			//	{
			//		throw;
			//	}
			//}
			return Ok(existingParent);
		}

		// POST: api/QrProfile
		[ResponseType(typeof(QrProfile))]
		[HttpPost]
		public async Task<IHttpActionResult> PostQrProfile(CreateProfileModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var profileToSave = new QrProfile();
			var user = await CreateUpdateUser(model);
			profileToSave.UserId = user.Id;

			var userId = this.User.Identity.GetUserId();
			profileToSave.Id = Guid.NewGuid();
			profileToSave.CreatedByUserId = userId;
			profileToSave.FirstName = model.FirstName;
			profileToSave.MiddleName = model.MiddleName;
			profileToSave.LastName = model.LastName;
			profileToSave.Sex = model.Sex;
			profileToSave.Birthday = model.Birthday.Value;
			profileToSave.DateOfDeath = model.DateOfDeath.Value;
			//profileToSave.Obituary = model.Obituary;
			db.QrProfiles.Add(profileToSave);
			db.UserToProfileRelationships.Add(new UserToProfileRelationship() { ProfileId = profileToSave.Id, UserId = user.Id });
			if (userId != user.Id)
			{
				db.UserToProfileRelationships.Add(new UserToProfileRelationship() { ProfileId = profileToSave.Id, UserId = userId });
			}
			//attempt payment before saving profile
			if (!String.IsNullOrEmpty(model.CardToken) || !User.IsInRole("Admin"))
			{
				var message = ChargeCard(model.CardToken);
				if (!String.IsNullOrEmpty(message))
				{
					return BadRequest(message);
				}
			}
			await db.SaveChangesAsync();

			var viewQrUrl = Url.Request.RequestUri.Scheme + "://" + Url.Request.RequestUri.Authority + Url.Route("QrManagement", null) + "/" + profileToSave.Id;

			var profileCreatedMessage = $@"
				<html>
					<body>
						<p>A new Embracing Memories profile has been created for {model.FirstName} {model.LastName}</p>
						<br />
						<p><a href=""{viewQrUrl}"">{viewQrUrl}</a></p>
						<br />
						<table>
							<tr>
								<td>{model.UserFirstName} {model.UserLastName}</td>
							</tr><tr>
								<td>{model.UserAddressLine1}</td>
							</tr><tr>
								<td>{model.UserAddressLine2}</td>
							</tr><tr>
								<td>{model.UserCity}, {model.UserState}&nbsp;&nbsp;{model.UserPostalCode}</td>
							</tr>
						</table>
						<br />
						{EmailService.Signature}
					</body>
				</html>
			";
			await EmailService.SendAsync(new EmailService.EmailMessage()
			{
				Body = profileCreatedMessage,
				Recipients = model.UserEmail,
				Subject = String.Format("Embracing Memories profile for {0} {1}", model.FirstName, model.LastName)
			});
			return CreatedAtRoute("ViewProfile", new { id = profileToSave.Id }, profileToSave);
		}

		private ReservedQrCode GetNextProfileId()
		{
			return db.ReservedQrCodes.Where(c => !c.Used).OrderBy(c => c.Key).FirstOrDefault();
		}

		private String ChargeCard(String token)
		{
			var myCharge = new StripeChargeCreateOptions();

			// always set these properties
			myCharge.Amount = (User.IsInRole("BasicUser") || User.IsInRole("Admin") || User.IsInRole("ProfileUser")) ? 19900 : User.IsInRole("ArchiveUser") ? 14900 : 19900;
			myCharge.Currency = "usd";

			// set this if you want to
			myCharge.Description = "Embracing Memories - New Profile";

			myCharge.SourceTokenOrExistingSourceId = token;

			// set this property if using a customer - this MUST be set if you are using an existing source!
			//myCharge.CustomerId = *customerId *;

			// set this if you have your own application fees (you must have your application configured first within Stripe)
			//myCharge.ApplicationFee = 25;

			// (not required) set this to false if you don't want to capture the charge yet - requires you call capture later
			myCharge.Capture = true;

			var chargeService = new StripeChargeService();
			try
			{
				StripeCharge stripeCharge = chargeService.Create(myCharge);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return null;
		}

		private Image GetQrImage(String qrUrl)
		{
			var url = "https://api.qrserver.com/v1/create-qr-code/?size=120x120&data=" + qrUrl;
			var webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
			webReq.Method = "GET";
			var webResponse = (HttpWebResponse)webReq.GetResponse();
			return Image.FromStream(webResponse.GetResponseStream());
		}

		private async Task<ApplicationUser> CreateUpdateUser(CreateProfileModel model)
		{
			var user = await UserManager.FindByEmailAsync(model.UserEmail);
			if (user == null)
			{
				user = new ApplicationUser()
				{
					UserName = model.UserEmail,
					Email = model.UserEmail,
					FirstName = model.UserFirstName,
					LastName = model.UserLastName,
					AddressLine1 = model.UserAddressLine1,
					AddressLine2 = model.UserAddressLine2,
					City = model.UserCity,
					PostalCode = model.UserPostalCode,
					State = model.UserState,
					Country = "US",
					PhoneNumber = model.UserPhone
				};
				var createUserResponse = await UserManager.CreateAsync(user);
				if (!createUserResponse.Succeeded)
				{
					throw new Exception(String.Join(Environment.NewLine, createUserResponse.Errors));
				}
				var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
				var callbackUrl = Url.Link("Default", new { controller = "Account", action = "ConfirmEmail", userId = user.Id, code = code });
				if (IsProd)
				{
					await UserManager.SendEmailAsync(user.Id,
					   "Confirm your Embracing Memories account", String.Format(@"
					<html>
						<body>
							<p>Please confirm your Embracing Memories account by clicking <a href=""{0}"">here</a></p>
							{1}
						</body>
					</html>", callbackUrl, EmailService.Signature));
				}
				else
				{
					await EmailService.SendAsync(new EmailService.EmailMessage()
					{
						Body = String.Format(@"
											<html>
												<body>
													<p>Please confirm your Embracing Memories account by clicking <a href=""{0}"">here</a></p>
													{1}
												</body>
											</html>", callbackUrl, EmailService.Signature),
						Subject = "Test - " + "Confirm your Embracing Memories account",
						DisplayName = "Me",
						Recipients = "rleeson_2000@yahoo.com",
						Sender = "support@embracingthememories.com"
					});
				}
			}
			else
			{
				user.FirstName = model.UserFirstName;
				user.LastName = model.UserLastName;
				user.AddressLine1 = model.UserAddressLine1;
				user.AddressLine2 = model.UserAddressLine2;
				user.City = model.UserCity;
				user.PostalCode = model.UserPostalCode;
				user.State = model.UserState;
				user.Country = "US";
				user.PhoneNumber = model.UserPhone;
				var updateUser = await UserManager.UpdateAsync(user);
				if (!updateUser.Succeeded)
				{
					throw new Exception(String.Join(Environment.NewLine, updateUser.Errors));
				}
			}
			await UserManager.AddToRoleAsync(user.Id, "ProfileUser");
			return user;
		}

		// DELETE: api/QrProfile/5
		[ResponseType(typeof(QrProfile))]
		[HttpDelete]
		[Authorize]
		public async Task<IHttpActionResult> DeleteQrProfile(int id)
		{
			var userId = this.User.Identity.GetUserId();
			QrProfile QrProfile = await db.QrProfiles.FindAsync(id);
			if (QrProfile == null)
			{
				return NotFound();
			}

			db.QrProfiles.Remove(QrProfile);
			await db.SaveChangesAsync();

			return Ok(QrProfile);
		}

		// GET: api/QrProfile/5
		[ResponseType(typeof(void))]
		[HttpPost]
		[Route("BindQrCode")]
		[Authorize(Roles = "Admin")]
		public async Task<IHttpActionResult> BindQrCode(BindQrCodeModel request)
		{
			var profiles = db.QrProfiles;
			var profileForQrCode = profiles.FirstOrDefault(p => p.QrCodeGuid == request.QrCode);
			if (profileForQrCode != null)
			{
				var middleName = !String.IsNullOrWhiteSpace(profileForQrCode.MiddleName) ? " " + profileForQrCode.MiddleName : String.Empty;
				return BadRequest($"This QR plaque has already been assigned to {profileForQrCode.FirstName}{middleName} {profileForQrCode.LastName}");
			}

			var QrProfile = await profiles.FindAsync(request.ProfileId);
			if (QrProfile == null)
			{
				return BadRequest("The profile was not found");
			}
			else if (QrProfile.QrCodeGuid != Guid.Empty)
			{
				return BadRequest("This profile is already bound to a QR plaque");
			}
			else
			{
				QrProfile.QrCodeGuid = request.QrCode;
				await db.SaveChangesAsync();
			}
			var createUser = UserManager.FindById(QrProfile.CreatedByUserId);
			var newQrUrl = Url.Request.RequestUri.Scheme + "://" + Url.Request.RequestUri.Authority + Url.Route("ViewProfile", new { id = request.QrCode });

			var qrImage = GetQrImage(newQrUrl.ToString());
			var ms = new MemoryStream();
			qrImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

			var profileCreatedMessage = $@"
				<html>
					<body>
						<p>Your profile for {QrProfile.FirstName} {QrProfile.LastName} is being processed.  Your physical QR Plaque will be mailed to the address below.  You will receive it in a few days.</p>
						<br />
						<table>
							<tr>
								<td>{createUser.AddressLine1}</td>
							</tr><tr>
								<td>{createUser.AddressLine2}</td>
							</tr><tr>
								<td>{createUser.City}, {createUser.State}&nbsp;&nbsp;{createUser.PostalCode}</td>
							</tr>
						</table>
						<p>The public can view your profile at: <a href=""{newQrUrl}"">{newQrUrl}</a></p>
						<p>Feel free to print the attached code as many times as you like.</p>
						<br />
						{EmailService.Signature}
					</body>
				</html>
			";
			await EmailService.SendAsync(new EmailService.EmailMessage()
			{
				Body = profileCreatedMessage,
				Recipients = createUser.Email,
				Subject = $"Embracing Memories profile for {QrProfile.FirstName} {QrProfile.LastName}",
				Attachment = new EmailService.Attachment()
				{
					Content = ms.ToArray(),
					Name = QrProfile.FirstName + " " + QrProfile.LastName + ".png"
				}
			});

			return Ok();
		}
		public class BindQrCodeModel
		{
			public Guid ProfileId { get; set; }
			public Guid QrCode { get; set; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool QrProfileExists(Guid id)
		{
			return db.QrProfiles.Count(e => e.Id == id) > 0;
		}
	}
}