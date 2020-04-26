using System.Collections.Generic;
using lab05.DTOs.Requests;
using lab05.Models;

namespace lab05.Services
{
    public interface IStudentsDbService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string id);
        public Enrollment AddStudent(Student student);
        public void DeleteStudent(int id);
        public void UpdateStudent(int id, Student student);
        public Enrollment PromoteStudents(PromoteStudentsRequest request);
    }
}
