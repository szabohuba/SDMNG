using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMNG.Models
{
    public class Contact : IdentityUser
    {
        public string FullName { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public bool Active { get;  set; }
        public string PWString { get; set; }
        
        public virtual Bus Bus { get; set; }

        
        // Navigation properties
        public ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}
