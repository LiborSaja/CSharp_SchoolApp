using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.DTO {
    public class SubjectDTO {
        public int Id { get; set; }
        [Required(ErrorMessage = "Název je povinný!")]
        [StringLength(25, ErrorMessage = "Jméno nesmí být delší, než 25 znaků!")]
        public string Name { get; set; }
    }
}
