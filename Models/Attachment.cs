using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMNG.Models
{
    public class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }

        // Foreign key
        public string? ContactId { get; set; }

        // Navigation property
        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }

        // Foreign key
        public string? BusId { get; set; }

        // Navigation property
        [ForeignKey("BusId")]
        public Bus Bus { get; set; }
    }
}
