using System.ComponentModel;
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
        [DisplayName("Имя")]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(150)]
        [DisplayName("Фамилия")]
        public string LastName { get; set; } = "";

        [StringLength(150)]
        [DisplayName("Отчество")]
        public string Patronymic { get; set; } = "";

        [StringLength(150)]
        [DisplayName("Псевдоним")]
        public string Pseudonym { get; set; } = "";

        [DisplayName("Дата рождения")]
        public DateOnly Birthday { get;set; }

        [ForeignKey("Country")]
        public long CountryId { get; set; }

        [DisplayName("Страна")]
        public Country Country { get; set; }

        [ForeignKey("Sex")]
        public long SexId { get; set; }

        [DisplayName("Пол")]
        public Sex Sex { get; set; }

        public virtual List<Exhibit>? Exhibits { get; set; } = new List<Exhibit>();
    }
}
