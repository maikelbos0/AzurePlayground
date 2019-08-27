using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzurePlayground.Domain {
    [Table("Users", Schema = "Security")]
    public class User {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
    }
}