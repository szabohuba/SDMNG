using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMNG.Models
{
    public class RouteStop
    {
        [Key]
        public string RouteStopId { get; set; }

        public string RoutStopName { get; set; }



        public string TransportRouteId { get; set; }

        [ForeignKey("TransportRouteId")]
        public TransportRoute TransportRoute { get; set; }


        public string StopId { get; set; }

        [ForeignKey("StopId")]
        public Stop Stop { get; set; }

        public int SequenceNumber { get; set; }

    }
}
