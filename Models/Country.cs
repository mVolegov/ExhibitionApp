using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExhibitionApp.Models
{
    public class Country
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Краткое наименование")]
        public string ShortName { get; set; } = "";

        public virtual List<Author> Authors { get; set; } = new List<Author>();
    }
}
