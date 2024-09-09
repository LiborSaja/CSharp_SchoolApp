using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.DTO {
    public class StudentDTO {
        public int Id { get; set; }
        [Required(ErrorMessage ="Jméno je povinné!")]
        [StringLength(25, ErrorMessage = "Jméno nesmí být delší, než 25 znaků!")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Příjmení je povinné!")]
        [StringLength(25, ErrorMessage = "Příjmení nesmí být delší, než 25 znaků!")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Datum narození je povinné!")]
        public DateTime DateOfBirth { get; set; }
    }
}
