using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.ViewModels {
    public class UserViewModel {

        [Required(ErrorMessage = "Přihlašovací jméno je povinné.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Přihlašovací jméno může obsahovat pouze písmena a číslice. Nepoužívejte, prosíme, znaky ani diakritiku.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email je povinný.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Nesprávný formát emailu")]   //regulární výraz
        public string Email { get; set; }
        [Required(ErrorMessage = "Heslo je povinné.")]
        [CustomPasswordValidator]
        public string Password { get; set; }

    }
}
