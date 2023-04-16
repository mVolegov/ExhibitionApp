using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ExhibitionApp.Models
{
    public class User
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина имени должна быть от 2 до 50 символов")]
        [Required(ErrorMessage = "Необходимо указать имя пользователя")]
        public string Name { get; set; } = null!;

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Необходимо указать пароль")]
        [
            RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,20}$", 
            ErrorMessage = 
                "Длина пароля должна быть от 8 до 20 символов. Также он должен содержать цифры и заглавные буквы")
        ]
        public string PasswordHash { get; set; } = null!;

        [Display(Name = "Администратор")]
        public bool IsAdmin { get; set; }
    }
}
