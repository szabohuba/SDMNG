using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMNG.Models
{
    public class TransportRoute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TransportRoutesId { get; set; }

        [Required]
        [StringLength(100)]
        public string TransportRoutesName { get; set; }


        public Schedule Schedule { get; set; }

        public ICollection<RouteStop> RouteStop { get; set; }

    }
}