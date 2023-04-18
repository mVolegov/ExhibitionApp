using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExhibitionApp.Models
{
    public class Exhibit
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Название экспоната")]
        public string Name { get; set; } = "";

        [Required]
        [DisplayName("Дата создания")]
        public DateOnly CreationDate { get; set; }

        [Required]
        [DisplayName("Дата поступления")]
        public DateOnly ArrivalDate { get; set; }

        [ForeignKey("ExhibitType")]
        [DisplayName("Тип экспоната")]
        public long? ExhibitTypeId { get; set; }

        public virtual ExhibitType ExhibitType { get; set; }

        [DisplayName("Авторы")]
        public virtual List<Author> Authors { get; set; } = new List<Author>();
        
        [NotMapped]
        [DisplayName("Авторы")]
        public List<long> AuthorsId { get; set; } = new List<long>();

        public virtual List<Exhibition>? Exhibitions { get; set; } = new List<Exhibition>();

        [ForeignKey("Warehouse")]
        [DisplayName("Склад")]
        public long? WarehouseId { get; set; }

        public virtual Warehouse Warehouse { get; set; }
    }
}
