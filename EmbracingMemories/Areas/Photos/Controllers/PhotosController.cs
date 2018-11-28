using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EmbracingMemories.Areas.QrProfiles.Models;
using System.Web;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using EmbracingMemories.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using EmbracingMemories.Areas.Account.Models;
using EmbracingMemories.Models;
using EmbracingMemories.Providers;
using System.Reflection;
using System.Text;

namespace EmbracingMemories.Areas.Photos.Controllers
{
    [RoutePrefix("api/Photos")]
    public class PhotosController : ApiController
    {
        private QrContext db = new QrContext();

        // GET: api/Photos
        public IQueryable<Photo> GetPhotos()
        {
            return db.Photos;
        }

        // GET: api/Photos/5
        [ResponseType(typeof(IEnumerable<Photo>))]
        [Route("GetPhotos/{profileId}")]
        public async Task<IHttpActionResult> GetPhotos(Guid profileId)
        {
            IEnumerable<Photo> photos = await db.Photos.ToArrayAsync();
            photos = photos.Where(p => p.QrProfileId == profileId);
            if (photos == null)
            {
                return NotFound();
            }

            return Ok(photos);
        }

        // GET: api/Photos/5
        [ResponseType(typeof(Photo))]
        public async Task<IHttpActionResult> GetPhoto(Guid id)
        {
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            return Ok(photo);
        }

        // POST: api/Photos
        [ResponseType(typeof(Photo))]
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
                var result = new List<Photo>();
                var docfiles = new List<string>();
                foreach (var fileData in tsk.FileData)
                {
                    Photo photo = null;
                    // Sometimes the filename has a leading and trailing double-quote character
                    // when uploaded, so we trim it; otherwise, we get an illegal character exception
                    var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
                    var mediaType = fileData.Headers.ContentType.MediaType;
                    var localFileName = fileData.LocalFileName;
                    using (var fileStream = File.OpenRead(fileData.LocalFileName))
                    {
                        if (IsValidFile(fileStream))
                        {
                            fileStream.Position = 0;
                            using (var tinyPng = ResizeImage(fileStream))
                            {
                                photo = new Photo();
                                photo.QrProfileId = profileId;
                                photo.UploadedByUserId = this.User.Identity.GetUserId();
                                photo.UploadedOn = DateTime.Now;
                                var image = Image.FromStream(fileStream);
                                if (image.RawFormat.Equals(ImageFormat.Png))
                                {
                                    photo.Extension = ".png";
                                }
                                else if (image.RawFormat.Equals(ImageFormat.Jpeg))
                                {
                                    photo.Extension = ".jpg";
                                }
                                db.Photos.Add(photo);
                                var service = new BlobService();
                                await service.UploadBlob(photo.Id.ToString(), mediaType, tinyPng, BlobHelper.Repository.Photos );
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    File.Delete(fileData.LocalFileName);
                }
                return Ok();
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Photos/5
        [ResponseType(typeof(Byte[]))]
        [Route("Content/{id}")]
        public async Task<IHttpActionResult> GetPhotoContent(Guid id)
        {
            var photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            var blobService = new BlobService();
            var result = await blobService.DownloadBlob(photo.Id.ToString(), BlobHelper.Repository.Photos );

            // Reset the stream position; otherwise, download will not work
            result.BlobStream.Position = 0;

            var filePath = Path.GetTempPath() + id;
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                result.BlobStream.WriteTo(fs);
                result.BlobStream.Close();
                fs.Close();
            }
            return new FileResult(filePath, "image/" + photo.Extension.Replace(".", "").Replace("jpg", "jpeg"));
        }

        // DELETE: api/Photos/5
        [ResponseType(typeof(Photo))]
        [Authorize]
        [Route("Delete/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeletePhoto(Guid id)
        {
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            db.Photos.Remove(photo);
            await db.SaveChangesAsync();

            return Ok(photo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PhotoExists(Guid id)
        {
            return db.Photos.Count(e => e.Id == id) > 0;
        }

        //or ValidateFileAttribute : RequiredAttribute
        public bool IsValidFile(FileStream file)
        {
            if (file == null)
            {
                return false;
            }

            //if (file.Length > 1 * 1024 * 1024)
            //{
            //    return false;
            //}

            try
            {
                using (var img = Image.FromStream(file))
                {
                    return img.RawFormat.Equals(ImageFormat.Png) || img.RawFormat.Equals(ImageFormat.Jpeg);
                }
            }
            catch { }
            return false;
        }

        public FileStream ResizeImage(FileStream fs)
        {
            var key = "kyg3fSED1o7jHVCeWBhCCeFvntXTZbOS";

            var url = "https://api.tinify.com/shrink";

            var req = (HttpWebRequest)WebRequest.Create(url);
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + key));
            req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);

            var output = String.Empty;
            req.Method = "POST"; // you might use "POST"
            req.ContentLength = fs.Length;
            req.AllowWriteStreamBuffering = true;
            Stream reqStream = req.GetRequestStream();

            var inData = new byte[fs.Length];
            // Get data from upload file to inData
            int bytesRead = fs.Read(inData, 0, (Int32)fs.Length);
            // put data into request stream
            reqStream.Write(inData, 0, (Int32)fs.Length);

            var urlForResize = req.GetResponse().Headers["Location"].ToString();
            req = (HttpWebRequest)WebRequest.Create(urlForResize);
            req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);
            req.Method = "POST"; // you might use "POST"
            req.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                var json = "{\"resize\":{ \"method\": \"fit\",\"height\": 500, \"width\": 500 } }";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var response = req.GetResponse();
            var readStream = response.GetResponseStream();
            output = String.Format("{0}tiny-output{1}.jpg", Path.GetTempPath(), DateTime.Now.ToString("-yyyy-MM-dd-HHmmss"));
            using (var fileStream = File.OpenWrite(output))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        var buffer = new byte[256];
                        count = readStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, count);

                    } while (count > 0);
                    var file = memoryStream.ToArray();
                    fileStream.Write(file, 0, file.Length);
                }
            }
            response.Close();
            return File.OpenRead(output);
        }
    }
}