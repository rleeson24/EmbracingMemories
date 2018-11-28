using System;
using System.ComponentModel.DataAnnotations;

namespace EmbracingMemories.Areas.Archive.Models
{
    public class CreateArchiveEntryModel
    {
        [Required]
        public String FirstName { get; set; }
        [Required]
        public String LastName { get; set; }
        public String MiddleName { get; set; }
        public String Suffix { get; set; }
    }
}