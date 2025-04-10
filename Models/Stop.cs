using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMNG.Models
{
    public class Stop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string StopId { get; set; }


        [Required]
        [StringLength(100)]
        public string StopName { get; set; }
        [Required]
        [StringLength(20)]
        public decimal Latitude { get; set; }
        [Required]
        [StringLength(20)]
        public decimal Longitude { get; set; }

        public string TransportRouteId { get; set; }
        [ForeignKey("TransportRouteId")]
        public TransportRoute TransportRoute { get; set; }

    }
}
