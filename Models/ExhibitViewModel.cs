using System.ComponentModel;

namespace ExhibitionApp.Models
{
    public class ExhibitViewModel
    {
        public Exhibit Exhibit { get; set; }

        [DisplayName("������")]
        public List<long> SelectedAuthorsId { get; set; }
    }
}