using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExhibitionApp.Models
{
    public class Street
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Наименование улицы")]
        public string Name { get; set; } = "";

        [ForeignKey("City")]
        public long CityId { get; set; }

        [DisplayName("Город")]
        public City City { get; set; }

        public List<Address> Addresses { get; set; } = new List<Address>();
    }
}
