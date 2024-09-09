using System.ComponentModel.DataAnnotations;

public class CustomPasswordValidator : ValidationAttribute {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        if (value != null) {
            string password = value.ToString();            
            
            if (password.Length < 8) {
                return new ValidationResult("Heslo musí mít alespoň 8 znaků.");
            }
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) {
                return new ValidationResult("Heslo musí obsahovat alespoň jedno velké písmeno ('A'-'Z').");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]")) {
                return new ValidationResult("Heslo musí obsahovat alespoň jednu číslici ('0'-'9').");
            }

            //if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[^a-zA-Z0-9]")) {
            //    return new ValidationResult("Heslo musí obsahovat alespoň jeden speciální znak.");
            //}
        }
        return ValidationResult.Success;
    }
}
