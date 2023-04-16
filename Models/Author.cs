using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExhibitionApp.Models
{
    public class Author
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(150)]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(150)]
        public string LastName { get; set; } = "";

        [StringLength(150)]
        public string Patronymic { get; set; } = "";

        [StringLength(150)]
        public string Pseudonym { get; set; } = "";

        public DateOnly Birthday { get;set; }

        [ForeignKey("Country")]
        public long CountryId { get; set; }

        public Country Country { get; set; }

        [ForeignKey("Sex")]
        public long SexId { get; set; }

        public Sex Sex { get; set; }

        public virtual List<Exhibit> Exhibits { get; set; } = new List<Exhibit>();
    }
}
