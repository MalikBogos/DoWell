// Models/User.cs (Extra tabel voor de vereisten)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoWell.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navigation property
        public virtual ICollection<UserWorkbook> UserWorkbooks { get; set; }

        public User()
        {
            UserWorkbooks = new HashSet<UserWorkbook>();
            CreatedDate = DateTime.Now;
        }
    }
}