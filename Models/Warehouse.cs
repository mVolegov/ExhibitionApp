using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExhibitionApp.Models
{
    public class Warehouse
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [DisplayName("Общая площадь (.кв м)")]
        public double Area { get; set; }

        [ForeignKey("Address")]
        [DisplayName("Адрес")]
        public long AddressId { get; set; }

        public virtual Address Address { get; set; }

        public virtual List<Exhibit>? Exhibits { get; set; } = new List<Exhibit>();

        public override string ToString()
        {
            return Address.ToString();
        }
    }
}
