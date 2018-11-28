using EmbracingMemories.Areas.QrProfiles.Models;
using EmbracingMemories.Models;
using EmbracingMemories.Providers;
using EmbracingMemories.Utilities;
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
using VimeoDotNet.Net;

namespace EmbracingMemories.Areas.Video.Controllers
{
	[RoutePrefix("api/Video")]
	public class VideoController : ApiController
	{
		private QrContext db = new QrContext();
		private Boolean IsProd
		{
			get
			{
				return !HttpContext.Current.Request.Url.Host.Contains("localhost");
			}
		}


		// GET: api/Photos
		public IQueryable<VideoFile> GetVideo()
		{
			return db.Videos;
		}

		// GET: api/Photos/5
		[ResponseType(typeof(IEnumerable<VideoFile>))]
		[Route("GetVideo/{profileId}")]
		public async Task<IHttpActionResult> GetVideos(Guid profileId)
		{
			IEnumerable<VideoFile> files = await db.Videos.ToArrayAsync();
			files = files.Where(p => p.QrProfileId == profileId);
			if (files == null)
			{
				return NotFound();
			}

			return Ok(files);
		}

		// GET: api/Photos/5
		[ResponseType(typeof(VideoFile))]
		public async Task<IHttpActionResult> GetVideo(Guid id)
		{
			VideoFile VideoFile = await db.Videos.FindAsync(id);
			if (VideoFile == null)
			{
				return NotFound();
			}

			return Ok(VideoFile);
		}

		// POST: api/Photos
		[ResponseType(typeof(VideoFile))]
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
				var hasVideoAlbum = db.Videos.Any(q => q.QrProfileId == profileId);
				var profile = db.QrProfiles.First(q => q.Id == profileId);
				var profileName = profile.FirstName + " " + profile.LastName;

				var provider = new MultipartFileStreamProvider(Path.GetTempPath());
				var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
				foreach (var header in Request.Content.Headers)
				{
					content.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}
				var tsk = await content.ReadAsMultipartAsync(provider);
				var result = new List<VideoFile>();
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

						videoFile = new VideoFile();
						videoFile.QrProfileId = profileId;
						videoFile.UploadedByUserId = this.User.Identity.GetUserId();
						videoFile.UploadedOn = DateTime.Now;
						videoFile.Extension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.Replace("\"", ""));
						videoFile.Url = uploadRequest.ClipUri.Replace("videos", "video");
						videoFile.ClipId = uploadRequest.ClipId.Value;

						db.Videos.Add(videoFile);

						//var service = new BlobService();
						//await service.UploadBlob( videoFile.Id.ToString(), mediaType, fileStream, BlobHelper.Repository.Video );
						await db.SaveChangesAsync();
					}
					result.Add(videoFile);
					File.Delete(fileData.LocalFileName);
				}
				return Ok(result);
			}
			catch (InvalidDataException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE: api/Photos/5
		[ResponseType(typeof(Byte[]))]
		[Route("Content/{id}")]
		public async Task<IHttpActionResult> GetVideoContent(Guid id)
		{
			var VideoFile = await db.Videos.FindAsync(id);
			if (VideoFile == null)
			{
				return NotFound();
			}

			var blobService = new BlobService();
			var result = await blobService.DownloadBlob(VideoFile.Id.ToString(), BlobHelper.Repository.Video);

			// Reset the stream position; otherwise, download will not work
			result.BlobStream.Position = 0;

			var filePath = Path.GetTempPath() + id;
			using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
			{
				result.BlobStream.WriteTo(fs);
				result.BlobStream.Close();
				fs.Close();
			}
			return new FileResult(filePath, "application/octet-stream");
		}

		// DELETE: api/Photos/5
		[ResponseType(typeof(Photo))]
		[Authorize]
		[Route("Delete/{id}")]
		[HttpDelete]
		public async Task<IHttpActionResult> DeleteVideoFile(Guid id)
		{
			VideoFile VideoFile = await db.Videos.FindAsync(id);
			if (VideoFile == null)
			{
				return NotFound();
			}
			//var client = new VimeoDotNet.VimeoClient("101df01f949f134370f77a375d8e169e7cc43ae8", "9rZXXj+4iRk8Wv95QPNNjf4sU+MI9+O7CczWnRp4L1dQF0V7hGeHA/bR0repqePh+TyvZNnRW2aFlWd34RD5JCfp7clj8nQuFZHRTxYjeSXwutIyJOSW/G9Sv5athNfB");
			var client = new VimeoDotNet.VimeoClient("7510a353d5691dd4d17f90455d063329");
			await client.DeleteVideoAsync(VideoFile.ClipId);
			db.Videos.Remove(VideoFile);
			await db.SaveChangesAsync();

			return Ok(VideoFile);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool VideoFileExists(Guid id)
		{
			return db.Videos.Count(e => e.Id == id) > 0;
		}

	}
}
