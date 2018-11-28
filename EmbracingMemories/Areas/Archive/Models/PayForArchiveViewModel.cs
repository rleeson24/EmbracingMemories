using System.ComponentModel.DataAnnotations;

namespace EmbracingMemories.Areas.Archive.Models
{
    public class PayForArchiveViewModel
    {
        [Required]
        public string CardToken { get; set; }
    }
}