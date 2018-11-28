using EmbracingMemories.Areas.QrProfiles.Models;
using EmbracingMemories.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace EmbracingMemories.Areas.QrProfiles.Controllers
{
	public class ViewProfileController : ApiController
	{
		private QrContext db = new QrContext();

		// GET: api/ViewProfile
		public IQueryable<QrProfile> GetQrProfiles()
		{
			return db.QrProfiles;
		}

		// GET: api/ViewProfile/5
		[ResponseType(typeof(QrProfile))]
		public async Task<IHttpActionResult> GetQrProfile(int id)
		{
			QrProfile qrProfile = await db.QrProfiles.FindAsync(id);

			return Ok(qrProfile);
		}

		// PUT: api/ViewProfile/5
		[ResponseType(typeof(void))]
		public async Task<IHttpActionResult> PutQrProfile(Guid id, QrProfile qrProfile)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != qrProfile.Id)
			{
				return BadRequest();
			}

			db.Entry(qrProfile).State = EntityState.Modified;

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

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/ViewProfile
		[ResponseType(typeof(QrProfile))]
		public async Task<IHttpActionResult> PostQrProfile(QrProfile qrProfile)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.QrProfiles.Add(qrProfile);
			await db.SaveChangesAsync();

			return CreatedAtRoute("DefaultApi", new { id = qrProfile.Id }, qrProfile);
		}

		// DELETE: api/ViewProfile/5
		[ResponseType(typeof(QrProfile))]
		public async Task<IHttpActionResult> DeleteQrProfile(int id)
		{
			QrProfile qrProfile = await db.QrProfiles.FindAsync(id);
			if (qrProfile == null)
			{
				return NotFound();
			}

			db.QrProfiles.Remove(qrProfile);
			await db.SaveChangesAsync();

			return Ok(qrProfile);
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