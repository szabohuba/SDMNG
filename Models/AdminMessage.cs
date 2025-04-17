using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMNG.Models
{
    public class AdminMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string adminmassegesId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string userName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string userEmail { get; set;}

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }

        public DateTime SentAt { get; set; }
        public DateTime ReadAt { get; set; }


        public bool IsRead { get; set; } 

    }

}
