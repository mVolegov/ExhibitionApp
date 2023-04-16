using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExhibitionApp.Models
{
    public class ExhibitType
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Тип экспоната")]
        public string TypeName { get; set; } = "";

        public virtual List<Exhibit> Exhibits { get; set; } = new List<Exhibit>();
    }
}
