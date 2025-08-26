// Models/FormatTemplate.cs
using System.ComponentModel.DataAnnotations;

namespace DoWell.Models
{
    public class FormatTemplate
    {
        [Key]
        public int FormatTemplateId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "Default Style";

        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }

        [StringLength(7)]
        public string BackgroundColor { get; set; } = "#FFFFFF";

        [StringLength(7)]
        public string ForegroundColor { get; set; } = "#000000";

        [StringLength(50)]
        public string FontFamily { get; set; } = "Segoe UI";

        public double FontSize { get; set; } = 11;

        // Navigation property for Workbook
        public int WorkbookId { get; set; }
        public virtual Workbook Workbook { get; set; } = null!;

        // Navigation property for Cells using this template
        public virtual ICollection<Cell> Cells { get; set; } = new List<Cell>();
    }
}