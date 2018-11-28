using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
    public class Document
    {
        public Document()
        {
            FileName = Guid.NewGuid();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        [Required]
        public Guid QrProfileId { get; set; }
        public String Label { get; set; }
        public DateTime UploadedOn { get; set; }
        public String UploadedByUserId { get; set; }
        public Guid FileName { get; set; }
    }
}