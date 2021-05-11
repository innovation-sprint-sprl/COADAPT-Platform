using System.ComponentModel.DataAnnotations;

namespace ApiModels {
    public class AppUsageLogRequest {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Tag { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
