using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SchoolWebApp6_24.Models {
    public class Subject {
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
    }
}
