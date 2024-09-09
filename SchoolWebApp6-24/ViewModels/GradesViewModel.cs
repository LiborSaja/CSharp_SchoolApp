using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.ViewModels {
    public class GradesViewModel {
        public int Id { get; set; }       
        public string StudentName { get; set; }
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Téma je povinné.")]
        public string Topic { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Známka je povinná.")]
        public int Mark { get; set; }
    }
}
