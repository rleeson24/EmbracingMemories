using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EmbracingMemories.Areas.Archive.Models;
using EmbracingMemories.Models;
using Stripe;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using EmbracingMemories.Areas.QrProfiles.Models;
using System.IO;
using VimeoDotNet.Net;
using System.Threading.Tasks;

namespace EmbracingMemories.Areas.Archive.Controllers
{
	[RoutePrefix("api/Archive")]
	public class ArchiveController : ApiController
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

		// GET: api/ArchiveEntries
		[HttpGet]
		[Authorize]
		public IQueryable<ArchiveEntry> GetArchiveEntries()
		{
			return db.ArchiveEntries;
		}

		// GET: api/ArchiveEntries/5
		[ResponseType(typeof(ArchiveEntry))]
		[HttpGet]
		[Authorize]
		public IHttpActionResult GetArchiveEntry(Guid id)
		{
			ArchiveEntry archiveEntry = db.ArchiveEntries.Find(id);
			if (archiveEntry == null)
			{
				return NotFound();
			}

			return Ok(archiveEntry);
		}

		// PUT: api/ArchiveEntries/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutArchiveEntry(Guid id, ArchiveEntry archiveEntry)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != archiveEntry.Id)
			{
				return BadRequest();
			}

			db.Entry(archiveEntry).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ArchiveEntryExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/PayForArchiveFeature
		[ResponseType(typeof(ArchiveEntry))]
		[Route("PayForArchiveFeature")]
		public IHttpActionResult PayForArchiveFeature(PayForArchiveViewModel archiveEntry)
		{
			var userId = this.User.Identity.GetUserId();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var message = ChargeCard(archiveEntry.CardToken);
			if (!String.IsNullOrEmpty(message))
			{
				return BadRequest(message);
			}

			var result = UserManager.AddToRole(userId, "ArchiveUser");

			return Ok();
		}

		// POST: api/ArchiveEntries
		[ResponseType(typeof(ArchiveEntry))]
		public IHttpActionResult PostArchiveEntry(CreateArchiveEntryModel archiveEntry)
		{
			var userId = this.User.Identity.GetUserId();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var newEntry = new ArchiveEntry();
			newEntry.FirstName = archiveEntry.FirstName;
			newEntry.MiddleName = archiveEntry.MiddleName;
			newEntry.LastName = archiveEntry.LastName;
			newEntry.Suffix = archiveEntry.Suffix;
			newEntry.UserId = userId;
			db.ArchiveEntries.Add(newEntry);

			try
			{
				db.SaveChanges();
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}

			return Created(@"~\Archive", newEntry);
		}

		// DELETE: api/ArchiveEntries/5
		[ResponseType(typeof(ArchiveEntry))]
		public IHttpActionResult DeleteArchiveEntry(Guid id)
		{
			ArchiveEntry archiveEntry = db.ArchiveEntries.Find(id);
			if (archiveEntry == null)
			{
				return NotFound();
			}

			db.ArchiveEntries.Remove(archiveEntry);
			db.SaveChanges();

			return Ok(archiveEntry);
		}
		

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool ArchiveEntryExists(Guid id)
		{
			return db.ArchiveEntries.Count(e => e.Id == id) > 0;
		}

		private String ChargeCard(String token)
		{
			var myCharge = new StripeChargeCreateOptions();

			// always set these properties
			myCharge.Amount = User.IsInRole("ProfileUser") ? 9900 : 4900;
			myCharge.Currency = "usd";

			// set this if you want to
			myCharge.Description = "Embracing Memories - Archive Feature";

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



		// POST: api/Photos
		[ResponseType(typeof(VideoFile))]
		[Authorize]
		[Route("UploadVideo/{archiveEntryId}")]
		public async Task<IHttpActionResult> UploadVideo(Guid archiveEntryId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var entry = db.ArchiveEntries.First(q => q.Id == archiveEntryId);
				var profileName = entry.FirstName + " " + entry.LastName;

				var provider = new MultipartFileStreamProvider(Path.GetTempPath());
				var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
				foreach (var header in Request.Content.Headers)
				{
					content.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}
				var tsk = await content.ReadAsMultipartAsync(provider);
				var docfiles = new List<string>();
				foreach (var fileData in tsk.FileData)
				{
					VideoFile videoFile = null;
					// Sometimes the filename has a leading and trailing double-quote character
					// when uploaded, so we trim it; otherwise, we get an illegal character exception
					var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
					var mediaType = fileData.Headers.ContentType.MediaType;
					var localFileName = fileData.LocalFileName;
					using (var fileStream = File.OpenRead(fileData.LocalFileName))
					{
						fileStream.Position = 0;

						//var client = new VimeoDotNet.VimeoClient("101df01f949f134370f77a375d8e169e7cc43ae8", "9rZXXj+4iRk8Wv95QPNNjf4sU+MI9+O7CczWnRp4L1dQF0V7hGeHA/bR0repqePh+TyvZNnRW2aFlWd34RD5JCfp7clj8nQuFZHRTxYjeSXwutIyJOSW/G9Sv5athNfB");
						var client = new VimeoDotNet.VimeoClient("7510a353d5691dd4d17f90455d063329");
						long? videoAlbumId = IsProd ? 4572966 : 4573995;
						//if (!hasVideoAlbum)
						//{
						//    var editAlbumParameters = new VimeoDotNet.Parameters.EditAlbumParameters()
						//    {
						//        Description = $"Album for {profileName}",
						//        Name = profileName,
						//        Privacy = VimeoDotNet.Parameters.EditAlbumPrivacyOption.Anybody
						//    };
						//    var album = await client.CreateAlbumAsync(editAlbumParameters);
						//    profile.VideoAlbumId = album.GetAlbumId().Value;
						//}
						//videoAlbumId = profile.VideoAlbumId;

						var uploadRequest = await client.UploadEntireFileAsync(new BinaryContent(fileStream, fileData.Headers.ContentType.MediaType));
						await client.AddToAlbumAsync(videoAlbumId.Value, uploadRequest.ClipId.Value);

						entry.VideoUrl = uploadRequest.ClipUri.Replace("videos", "video");

						//var service = new BlobService();
						//await service.UploadBlob( videoFile.Id.ToString(), mediaType, fileStream, BlobHelper.Repository.Video );
						await db.SaveChangesAsync();
					}
					File.Delete(fileData.LocalFileName);
				}
				return Ok(entry);
			}
			catch (InvalidDataException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}