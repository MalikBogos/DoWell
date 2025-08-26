// Models/Workbook.cs
using System.ComponentModel.DataAnnotations;

namespace DoWell.Models
{
    public class Workbook
    {
        [Key]
        public int WorkbookId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = "New Workbook";

        [StringLength(500)]
        public string? FilePath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastSavedDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string Author { get; set; } = Environment.UserName;

        // Navigation property for Worksheets
        public virtual ICollection<Worksheet> Worksheets { get; set; } = new List<Worksheet>();

        // Navigation property for Format Templates
        public virtual ICollection<FormatTemplate> FormatTemplates { get; set; } = new List<FormatTemplate>();
    }
}