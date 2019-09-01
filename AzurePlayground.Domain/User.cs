using System.Collections.Generic;
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
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }
        [MaxLength(20)]
        public byte[] PasswordSalt { get; set; }
        [MaxLength(20)]
        public byte[] PasswordHash { get; set; }
        public int PasswordHashIterations { get; set; }
        public virtual ICollection<UserEvent> UserEvents { get; set; }
    }
}