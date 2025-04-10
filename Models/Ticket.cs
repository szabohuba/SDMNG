using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDMNG.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TicketId { get; set; }

        public DateTime PurchaseDate { get; set; }

        [StringLength(10)]
        public string SeatNumber { get; set; }


        /// Foreign keyf for connection

        /// Schedule
        
        public string ScheduleId { get; set; }

        [ForeignKey ("ScheduleId")]
        public Schedule Schedule { get; set; }


        // Passanger
        public string ContactId { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }
    }
}
