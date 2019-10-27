using System;

namespace AzurePlayground.Models.Security {
    public sealed class UserViewModel {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public  string StartDateString { get { return StartDate?.ToShortDateString(); } }
    }
}