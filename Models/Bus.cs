using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDMNG.Models
{
    public class Bus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BusId { get; set; }

        [Required]
        [StringLength(50)]
        public string BusNumber { get; set; }

        [Required]
        public int Capacity { get; set; }

        [StringLength(20)]
        public string BusType { get; set; }

        public string ImageUrl { get; set; }


        //Relationships
        public string ContactId { set; get; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }


        public ICollection<Schedule> Schedules { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

    }
}
