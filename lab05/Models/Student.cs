using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab05.Models
{
    public class Student
    {
        public int IdStudent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Faculty { get; set; }
        public int Semester { get; set; }
    }
}
