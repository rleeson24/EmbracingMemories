using System;
using System.ComponentModel.DataAnnotations;

namespace EmbracingMemories.Areas.Archive.Models
{
	public class ArchiveEntry
	{
		public ArchiveEntry()
		{
			Id = Guid.NewGuid();
			UserId = String.Empty;
			FirstName = String.Empty;
			MiddleName = String.Empty;
			LastName = String.Empty;
			Suffix = String.Empty;
		}

		[Key, Required]
		public Guid Id { get; set; }

		[Required]
		public string UserId { get; internal set; }

		[Required]
		public String FirstName { get; set; }

		public String MiddleName { get; set; }

		[Required]
		public String LastName { get; set; }

		public String Suffix { get; set; }

		public String VideoUrl { get; set; }
	}
}