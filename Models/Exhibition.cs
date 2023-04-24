using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExhibitionApp.Models
{
    public class Exhibition
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [DisplayName("Название выставки")]
        public string Name { get; set; }

        [ForeignKey("Address")]
        public long AddressId { get; set; }

        [DisplayName("Адрес")]
        public virtual Address Address { get; set; }

        [Required]
        [DisplayName("Дата начала")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime HostingDate { get; set; }

        [Required]
        [DisplayName("Дата окончания")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime ExpirationDate { get; set; }

        public virtual List<Exhibit> Exhibits { get; set; } = new List<Exhibit>();
    }
}
