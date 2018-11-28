using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
    public class Photo
    {
        public Photo()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid QrProfileId { get; set; }
        //public Boolean IsMaster { get; set; }
        public DateTime UploadedOn { get; set; }
        public String UploadedByUserId { get; set; }
        public string Extension { get; internal set; }
    }
}