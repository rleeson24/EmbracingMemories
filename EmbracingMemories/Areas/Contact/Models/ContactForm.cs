using System.ComponentModel.DataAnnotations;

namespace EmbracingMemories.Areas.Contact.Models
{
    public class ContactForm
    {
        [Required]
        public string Message { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string Name { get; set; }
        //[Required]
        //public string Subject { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        public string EnquiryType { get; set; }
    }
}