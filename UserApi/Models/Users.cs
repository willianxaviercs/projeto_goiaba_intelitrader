using System;
using System.ComponentModel.DataAnnotations;

namespace UserApi.Models
{
    public class User
    {
        [Required]
        public string FirstName  { get; set; }
        
        public string SurName { get; set; }

        [Range(1, 128)]
        [Required]
        public int Age { get; set; }

        [Required]
        public DateTime CreationTime { get; private set; } = DateTime.Now;

        [Key]
        public Guid Id { get; private set; }
    }
}
