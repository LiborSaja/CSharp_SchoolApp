
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.ViewModels {
    public class EditRoleViewModel {
        public string Id { get; set; }
        [Required(ErrorMessage = "Název je povinný!")]
        [StringLength(25, ErrorMessage = "Jméno nesmí být delší, než 25 znaků!")]
        public string Name { get; set; }
    }
}
