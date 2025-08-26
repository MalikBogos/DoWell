// Models/Cell.cs
using System.ComponentModel.DataAnnotations;

namespace DoWell.Models
{
    public class Cell
    {
        [Key]
        public int CellId { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }

        public string Value { get; set; } = "";

        // Formatting properties
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }

        // Color properties
        public string BackgroundColor { get; set; } = "#FFFFFF";
        public string ForegroundColor { get; set; } = "#000000";

        // Navigation property for Format template
        public int? FormatTemplateId { get; set; }
        public virtual FormatTemplate? FormatTemplate { get; set; }

        // Navigation property for Workbook (direct relationship)
        public int WorkbookId { get; set; }
        public virtual Workbook Workbook { get; set; } = null!;

        public Cell()
        {
            Value = "";
            IsBold = false;
            IsItalic = false;
            IsUnderline = false;
            BackgroundColor = "#FFFFFF";
            ForegroundColor = "#000000";
            WorkbookId = 1; // Default workbook
        }
    }
}