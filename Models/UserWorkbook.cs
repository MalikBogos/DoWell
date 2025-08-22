// Models/UserWorkbook.cs (Koppeltabel tussen User en Workbook)
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoWell.Models
{
    public class UserWorkbook
    {
        [Key]
        public int UserWorkbookId { get; set; }

        public int UserId { get; set; }
        public int WorkbookId { get; set; }

        public DateTime SharedDate { get; set; }
        public bool CanEdit { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("WorkbookId")]
        public virtual Workbook Workbook { get; set; }

        public UserWorkbook()
        {
            SharedDate = DateTime.Now;
            CanEdit = false;
        }
    }
}