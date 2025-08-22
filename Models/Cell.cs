// Models/Cell.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoWell.Models
{
    public class Cell
    {
        [Key]
        public int CellId { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }

        public string Value { get; set; }

        // Formatting properties
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }

        // Foreign key
        public int WorksheetId { get; set; }

        // Navigation property
        [ForeignKey("WorksheetId")]
        public virtual Worksheet Worksheet { get; set; }
    }
}