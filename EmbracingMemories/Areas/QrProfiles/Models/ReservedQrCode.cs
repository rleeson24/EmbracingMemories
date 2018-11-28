using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
    public class ReservedQrCode
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Key { get; set; }
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Boolean Used { get; set; }
    }
}