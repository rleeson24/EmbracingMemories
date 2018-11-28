using EmbracingMemories.Areas.Archive.Models;
using EmbracingMemories.Areas.QrProfiles.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EmbracingMemories.Models
{
	public class QrContext : DbContext
	{
		// You can add custom code to this file. Changes will not be overwritten.
		// 
		// If you want Entity Framework to drop and regenerate your database
		// automatically whenever you change your model schema, please use data migrations.
		// For more information refer to the documentation:
		// http://msdn.microsoft.com/en-us/data/jj591621.aspx

		public QrContext() : base("name=QrContextConnection")
		{
		}

		public DbSet<UserToProfileRelationship> UserToProfileRelationships { get; set; }
		public DbSet<QrProfile> QrProfiles { get; set; }
		public DbSet<Photo> Photos { get; set; }

		public DbSet<ReservedQrCode> ReservedQrCodes { get; set; }
		public DbSet<AudioFile> AudioFiles { get; set; }
		public DbSet<VideoFile> Videos { get; set; }
		public DbSet<ArchiveEntry> ArchiveEntries { get; set; }
		public DbSet<QrLink> Links { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}
}
