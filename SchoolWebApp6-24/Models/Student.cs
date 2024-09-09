using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SchoolWebApp6_24.Models {
    public class Student {
        public int Id { get; set; }
        [NotNull]
        public String? FirstName { get; set; }
        [NotNull]
        public string? LastName { get; set;}
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public string FullName { get => $"{FirstName} {LastName}"; }
    }
}
