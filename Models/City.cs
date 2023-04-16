using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExhibitionApp.Models
{
    public class City
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Наименование города")]
        public string Name { get; set; } = "";

        public List<Street> Streets { get; set;} = new List<Street>();
    }
}
