using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExhibitionApp.Models
{
    public class Exhibition
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Address")]
        public long AddressId { get; set; }

        public virtual Address Address { get; set; }

        [Required]
        public DateTime HostingDate { get; set; }

        public virtual List<Exhibit> Exhibits { get; set; } = new List<Exhibit>();
    }
}
