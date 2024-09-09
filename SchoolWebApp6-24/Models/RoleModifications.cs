using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.Models {
    public class RoleModifications {
        
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string[] AddIds { get; set; }
        public string[] DeleteIds { get; set; }
    }
}
