using EmbracingMemories.Areas.Photos.Models;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace EmbracingMemories.Providers
{
	public class BlobService
	{
		public async Task<BlobUploadModel> UploadBlob(String key, String mediaType, FileStream fileStream, BlobHelper.Repository repo)
		{
			var ipAddress = GetIPAddress();
			var isDev = ipAddress.StartsWith("192.168");
			if (isDev)
				key = "dev-" + key;
			// Retrieve reference to a blob
			var blobContainer = BlobHelper.GetBlobContainer(repo);
			var blob = blobContainer.GetBlockBlobReference(key);

			// Set the blob content type
			blob.Properties.ContentType = mediaType;

			// Upload file into blob storage, basically copying it from local disk into Azure
			fileStream.Position = 0;
			blob.UploadFromStream(fileStream);

			// Create blob upload model with properties from blob info
			var blobUpload = new BlobUploadModel
			{
				FileName = blob.Name,
				FileUrl = blob.Uri.AbsoluteUri,
				FileSizeInBytes = blob.Properties.Length
			};
			return blobUpload;
		}

		public async Task<BlobDownloadModel> DownloadBlob(String blobName, BlobHelper.Repository repo)
		{
			var ipAddress = GetIPAddress();
			var isDev = ipAddress.StartsWith("192.168");
			if (isDev)
				blobName = "dev-" + blobName;
			// TODO: You must implement this helper method. It should retrieve blob info
			// from your database, based on the blobId. The record should contain the
			// blobName, which you should return as the result of this helper method.
			if (!String.IsNullOrEmpty(blobName))
			{
				var container = BlobHelper.GetBlobContainer(repo);
				var blob = container.GetBlockBlobReference(blobName);

				// Download the blob into a memory stream. Notice that we’re not putting the memory
				// stream in a using statement. This is because we need the stream to be open for the
				// API controller in order for the file to actually be downloadable. The closing and
				// disposing of the stream is handled by the Web API framework.
				var ms = new MemoryStream();
				await blob.DownloadToStreamAsync(ms);

				// Strip off any folder structure so the file name is just the file name
				var lastPos = blob.Name.LastIndexOf('/');
				var fileName = blob.Name.Substring(lastPos + 1, blob.Name.Length - lastPos - 1);

				// Build and return the download model with the blob stream and its relevant info
				var download = new BlobDownloadModel
				{
					BlobStream = ms,
					BlobFileName = fileName,
					BlobLength = blob.Properties.Length,
					BlobContentType = blob.Properties.ContentType
				};

				return download;
			}

			// Otherwise
			return null;
		}


		public Task DownloadToStream(String blobName, Stream str, BlobHelper.Repository repo)
		{
			var ipAddress = GetIPAddress();
			var isDev = ipAddress.StartsWith("192.168");
			if (isDev)
				blobName = "dev-" + blobName;
			// TODO: You must implement this helper method. It should retrieve blob info
			// from your database, based on the blobId. The record should contain the
			// blobName, which you should return as the result of this helper method.
			if (!String.IsNullOrEmpty(blobName))
			{
				var container = BlobHelper.GetBlobContainer(repo);
				var blob = container.GetBlockBlobReference(blobName);

				return blob.DownloadToStreamAsync(str);
			}
			return null;
		}

		/// <summary>
		/// method to get Client ip address
		/// </summary>
		/// <param name="GetLan"> set to true if want to get local(LAN) Connected ip address</param>
		/// <returns></returns>
		public static string GetIPAddress(bool GetLan = false)
		{
			string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

			if (String.IsNullOrEmpty(visitorIPAddress))
				visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

			if (string.IsNullOrEmpty(visitorIPAddress))
				visitorIPAddress = HttpContext.Current.Request.UserHostAddress;

			if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
			{
				GetLan = true;
				visitorIPAddress = string.Empty;
			}

			if (GetLan)
			{
				if (string.IsNullOrEmpty(visitorIPAddress))
				{
					//This is for Local(LAN) Connected ID Address
					string stringHostName = Dns.GetHostName();
					//Get Ip Host Entry
					IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
					//Get Ip Address From The Ip Host Entry Address List
					IPAddress[] arrIpAddress = ipHostEntries.AddressList;

					try
					{
						visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
					}
					catch
					{
						try
						{
							visitorIPAddress = arrIpAddress[0].ToString();
						}
						catch
						{
							try
							{
								arrIpAddress = Dns.GetHostAddresses(stringHostName);
								visitorIPAddress = arrIpAddress[0].ToString();
							}
							catch
							{
								visitorIPAddress = "127.0.0.1";
							}
						}
					}
				}
			}
			return visitorIPAddress;
		}
	}
}