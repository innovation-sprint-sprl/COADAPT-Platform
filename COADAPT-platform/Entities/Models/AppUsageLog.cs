using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class AppUsageLog {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "DATETIME")]
        public DateTime ReportedOn { get; set; }

        public int UserId { get; set; }

        public string Tag { get; set; }

        public string Message { get; set; }
    }
}
