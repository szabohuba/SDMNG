using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMNG.Models
{
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        [Required]
        public int TicketLeft { get; set; }

        public bool IsCompleted { get; set; }


        public string TransportRouteId { get; set; }
        [ForeignKey("TransportRouteId")]
        public TransportRoute TransportRoute { get; set; }

        public string BusId { get; set; }
        [ForeignKey("BusId")]
        public Bus Bus { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
