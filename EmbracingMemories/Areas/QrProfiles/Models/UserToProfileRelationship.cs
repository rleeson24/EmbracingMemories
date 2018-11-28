using EmbracingMemories.Areas.Account.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
    public class UserToProfileRelationship
    {
        [Key, Column(Order = 1)]

        public String UserId { get; set; }
        [Key, Column(Order = 2)]

        public Guid ProfileId { get; set; }
    }
}