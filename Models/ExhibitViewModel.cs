using System.ComponentModel;

namespace ExhibitionApp.Models
{
    public class ExhibitViewModel
    {
        public Exhibit Exhibit { get; set; }

        [DisplayName("Авторы")]
        public List<long> SelectedAuthorsId { get; set; }
    }
}