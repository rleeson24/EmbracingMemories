using EmbracingMemories.Areas.QrProfiles.Models;
using EmbracingMemories.Models;
using EmbracingMemories.Providers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace EmbracingMemories.Areas.Audio.Controllers
{
	[RoutePrefix("api/Audio")]
	public class AudioController : ApiController
	{
		private QrContext db = new QrContext();

		// GET: api/Photos
		public IQueryable<AudioFile> GetAudio()
		{
			return db.AudioFiles;
		}

		// GET: api/Photos/5
		[ResponseType(typeof(IEnumerable<AudioFile>))]
		[Route("GetAudio/{profileId}")]
		public async Task<IHttpActionResult> GetAudioFiles(Guid profileId)
		{
			IEnumerable<AudioFile> files = await db.AudioFiles.ToArrayAsync();
			files = files.Where(p => p.QrProfileId == profileId);
			if (files == null)
			{
				return NotFound();
			}

			return Ok(files);
		}

		// GET: api/Photos/5
		[ResponseType(typeof(AudioFile))]
		public async Task<IHttpActionResult> GetAudio(Guid id)
		{
			AudioFile audioFile = await db.AudioFiles.FindAsync(id);
			if (audioFile == null)
			{
				return NotFound();
			}

			return Ok(audioFile);
		}

		// POST: api/Photos
		[ResponseType(typeof(AudioFile))]
		[Authorize]
		[Route("Upload/{profileId}")]
		public async Task<IHttpActionResult> Upload(Guid profileId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var provider = new MultipartFileStreamProvider(Path.GetTempPath());
				var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
				foreach (var header in Request.Content.Headers)
				{
					content.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}
				var tsk = await content.ReadAsMultipartAsync(provider);
				var result = new List<AudioFile>();
				var docfiles = new List<string>();
				foreach (var fileData in tsk.FileData)
				{
					AudioFile audioFile = null;
					// Sometimes the filename has a leading and trailing double-quote character
					// when uploaded, so we trim it; otherwise, we get an illegal character exception
					var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
					var mediaType = fileData.Headers.ContentType.MediaType;
					var localFileName = fileData.LocalFileName;
					using (var fileStream = File.OpenRead(fileData.LocalFileName))
					{
						fileStream.Position = 0;
						audioFile = new AudioFile();
						audioFile.QrProfileId = profileId;
						audioFile.UploadedByUserId = this.User.Identity.GetUserId();
						audioFile.UploadedOn = DateTime.Now;
						audioFile.Extension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.Replace("\"", ""));
						db.AudioFiles.Add(audioFile);
						var service = new BlobService();
						await service.UploadBlob(audioFile.Id.ToString(), mediaType, fileStream, BlobHelper.Repository.Audio);
						await db.SaveChangesAsync();
					}
					result.Add(audioFile);
					File.Delete(fileData.LocalFileName);
				}
				return Ok(result);
			}
			catch (InvalidDataException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		//[ResponseType(typeof(Byte[]))]
		//[Route("Content/{id}")]
		//public async Task<IHttpActionResult> GetAudioContent(Guid id)
		//{
		//	var audioFile = await db.AudioFiles.FindAsync(id);
		//	if (audioFile == null)
		//	{
		//		return NotFound();
		//	}

		//	var blobService = new BlobService();
		//	var result = await blobService.DownloadBlob(audioFile.Id.ToString(), BlobHelper.Repository.Audio);

		//	// Reset the stream position; otherwise, download will not work
		//	result.BlobStream.Position = 0;

		//	var filePath = Path.GetTempPath() + id;
		//	using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
		//	{
		//		result.BlobStream.WriteTo(fs);
		//		result.BlobStream.Close();
		//		fs.Close();
		//	}
		//	return new FileResult(filePath, "application/octet-stream");// "audio/" + ( audioFile.Extension == ".mp3" ? "mpeg" : "x-ms-wma" ) );
		//}


		[ResponseType(typeof(Byte[]))]
		[Route("Content/{id}")]
		public HttpResponseMessage GetAudioContent(Guid id)
		{
			var response = Request.CreateResponse();

			var audioFile = db.AudioFiles.Find(id);
			if (audioFile == null)
			{
				response.StatusCode = System.Net.HttpStatusCode.NotFound;
				return response;
			}
			var blobService = new BlobService();

			response.Content = new PushStreamContent(
				async (outputStream, httpContent, transportContext) =>
				{
					try
					{
						// Execute the command and get a reader
						await blobService.DownloadToStream(audioFile.Id.ToString(), outputStream, BlobHelper.Repository.Audio);
					}
					catch (HttpException ex)
					{
						if (ex.ErrorCode == -2147023667) // The remote host closed the connection. 
						{
							return;
						}
					}
					finally
					{
						// Close output stream as we are done
						outputStream.Close();
					}
				});
			return response;
		}

		// DELETE: api/Photos/5
		[ResponseType(typeof(Photo))]
		[Authorize]
		[Route("Delete/{id}")]
		[HttpDelete]
		public async Task<IHttpActionResult> DeleteAudioFile(Guid id)
		{
			AudioFile audioFile = await db.AudioFiles.FindAsync(id);
			if (audioFile == null)
			{
				return NotFound();
			}

			db.AudioFiles.Remove(audioFile);
			await db.SaveChangesAsync();

			return Ok(audioFile);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool AudioFileExists(Guid id)
		{
			return db.AudioFiles.Count(e => e.Id == id) > 0;
		}

	}
}
