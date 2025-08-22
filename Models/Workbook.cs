// Models/Workbook.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoWell.Models
{
    public class Workbook
    {
        [Key]
        public int WorkbookId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // Navigation property
        public virtual ICollection<Worksheet> Worksheets { get; set; }

        public Workbook()
        {
            Worksheets = new HashSet<Worksheet>();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }
    }
}