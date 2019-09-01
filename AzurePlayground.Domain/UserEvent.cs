using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzurePlayground.Domain {
    [Table("UserEvents", Schema = "Security")]
    public class UserEvent {
        [Key]
        public int Id { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public UserEventType UserEventType { get; set; }
    }
}