// Models/Worksheet.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoWell.Models
{
    public class Worksheet
    {
        [Key]
        public int WorksheetId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        // Foreign key
        public int WorkbookId { get; set; }

        // Navigation properties
        [ForeignKey("WorkbookId")]
        public virtual Workbook Workbook { get; set; }
        public virtual ICollection<Cell> Cells { get; set; }

        public Worksheet()
        {
            Cells = new HashSet<Cell>();
            RowCount = 10;
            ColumnCount = 10;
        }
    }
}