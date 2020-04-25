using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lab05.DTOs
{
    public class EnrollStudentRequest
    {
     
        [Required]
        [RegularExpression("^s[0-9]+$")]
        [MaxLength(15)]
        public string IndexNumber { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }
    }
}
