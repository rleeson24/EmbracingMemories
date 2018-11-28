﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
    public class VideoFile
    {
        public VideoFile()
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
        public string Url { get; internal set; }
        public long ClipId { get; internal set; }
    }
}