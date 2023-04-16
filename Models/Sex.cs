using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExhibitionApp.Models
{
    public class Sex
    {
        [Key]
        [DisplayName("Код")]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";

        public virtual List<Author> Authors { get; set; } = new List<Author>();
    }
}
