// Models/Worksheet.cs
using System.ComponentModel.DataAnnotations;

namespace DoWell.Models
{
    public class Worksheet
    {
        [Key]
        public int WorksheetId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "Sheet1";

        public int TabOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation property for Workbook
        public int WorkbookId { get; set; }
        public virtual Workbook Workbook { get; set; } = null!;

        // Navigation property for Cells
        public virtual ICollection<Cell> Cells { get; set; } = new List<Cell>();
    }
}