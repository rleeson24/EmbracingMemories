using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
	public class QrProfile
	{
		public QrProfile()
		{
			CreateTimestamp = DateTime.Now;
			Id = Guid.NewGuid();
			Moments = new List<MemorableMoment>();
			Links = new List<QrLink>();
			Documents = new List<Document>();
			Users = new List<String>();
			FirstName = String.Empty;
			MiddleName = String.Empty;
			LastName = String.Empty;
			Obituary = String.Empty;
			LifeHistory = String.Empty;
		}

		[Key, Required]
		public Guid Id { get; set; }
		[Required]
		public String FirstName { get; set; }
		public String MiddleName { get; set; }
		[Required]
		public String LastName { get; set; }
		public DateTime Birthday { get; set; }
		public DateTime DateOfDeath { get; set; }
		[Required]
		public String Sex { get; set; }
		public String Obituary { get; set; }
		public String LifeHistory { get; set; }
		public string UserId { get; internal set; }
		public String CreatedByUserId { get; set; }

		[NotMapped]
		public IEnumerable<String> Users { get; set; }

		public virtual ICollection<MemorableMoment> Moments { get; set; }
		public virtual ICollection<QrLink> Links { get; set; }
		public virtual ICollection<Document> Documents { get; set; }
		public virtual ICollection<Photo> Photos { get; set; }
		public virtual ICollection<AudioFile> AudioFiles { get; set; }
		public virtual ICollection<VideoFile> Videos { get; set; }
		public long? VideoAlbumId { get; set; }
		public Guid QrCodeGuid { get; set; }
		public DateTime? CreateTimestamp { get; set; }
	}

	public class QrLink
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Int32? Id { get; set; }
		public Guid QrProfileId { get; set; }
		[Required]
		public String Label { get; set; }
		[Required]
		public String Url { get; set; }
	}
}