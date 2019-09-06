using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzurePlayground.Domain.Security {
    [Table("Users", Schema = "Security")]
    public class User {
        [Key]
        public int Id { get; set; }
        [Required]
        [Index("uq_Email", IsUnique = true)]
        [StringLength(255)]
        public string Email { get; set; }
        [MaxLength(20)]
        public byte[] PasswordSalt { get; set; }
        [MaxLength(20)]
        public byte[] PasswordHash { get; set; }
        public int PasswordHashIterations { get; set; }
        public int? ActivationCode { get; set; }
        public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}