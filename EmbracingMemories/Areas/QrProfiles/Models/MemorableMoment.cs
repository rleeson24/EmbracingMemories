using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
    public class MemorableMoment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        [Required]
        public Guid QrProfileId { get; set; }
        public DateTime? OccurredOn { get; set; }
        public string Text { get; set; }
    }
}