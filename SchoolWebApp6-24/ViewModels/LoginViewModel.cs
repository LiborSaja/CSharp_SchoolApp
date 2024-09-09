using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.ViewModels {
    public class LoginViewModel {

        [Required(ErrorMessage = "Přihlašovací jméno je povinné.")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Heslo je povinné!")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
        
    }
}
