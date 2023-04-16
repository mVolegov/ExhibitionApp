using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExhibitionApp.Models
{
    public class Address
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Street")]
        public long StreetId { get; set; }
        public Street Street { get; set; }

        [Required]
        [DisplayName("Номер дома")]
        public string HouseNumber { get; set; }

        public override string ToString()
        {
            return Street.City.Name + ", " + Street.Name + ", " + HouseNumber;
        }
    }
}
