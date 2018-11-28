using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EmbracingMemories.Providers
{
    public static class BlobHelper
    {
        public static CloudBlobContainer GetBlobContainer( Repository repo )
        {
            // Pull these from config
            var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
			var blobStorageContainerName = String.Empty;
			switch( repo )
			{
				case Repository.Photos:
					blobStorageContainerName = ConfigurationManager.AppSettings[ "PhotoBlobStorageContainerName" ];
					break;
				case Repository.Audio:
					blobStorageContainerName = ConfigurationManager.AppSettings[ "AudioBlobStorageContainerName" ];
					break;
                case Repository.Video:
                    blobStorageContainerName = ConfigurationManager.AppSettings[ "VideoBlobStorageContainerName" ];
                    break;
            }

            // Create blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(blobStorageContainerName);
        }
		public enum Repository
		{
			Photos,
			Audio,
            Video
        }
    }
}