using System.ComponentModel.DataAnnotations;

namespace SDMNG.Models
{
    public class AdminMessage
    {
        [Key]
        public string adminmassegesId { get; set; }
        [Required]
        public string userName { get; set; }

        [Required, EmailAddress]
        public string userEmail { get; set;}

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime SentAt { get; set; } 

        public bool IsRead { get; set; } 
    }

}
