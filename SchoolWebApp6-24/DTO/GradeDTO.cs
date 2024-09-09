using SchoolWebApp6_24.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.DTO {
    public class GradeDTO {
        public int Id { get; set; }
        [DisplayName("Jméno studenta")]
        public int StudentId { get; set; }
        [DisplayName("Jméno předmětu")]
        public int SubjectId { get; set; }
        [Required(ErrorMessage = "Téma je povinné.")]
        public string Topic { get; set; }
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Známka je povinná.")]
        public int Mark { get; set; }
    }
}
