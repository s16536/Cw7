using lab05.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace lab05.DAL
{
    public class MSSQLService : IDbService
    {
        private const string CONNECTION_STRING = "Data Source=db-mssql;Initial Catalog=s16536;Integrated Security=True";
        
        private const string SELECT_SQL = "select FirstName, LastName, BirthDate, Semester, Name from Student s left join Enrollment e on s.IdEnrollment = e.IdEnrollment left join Studies studies on e.IdStudy = studies.IdStudy";

        public void AddStudent(Student student)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudent(int id)
        {
            throw new NotImplementedException();
        }

        public Student GetStudent(string id)
        {
            //sql injection nie dziala
            var command = new SqlCommand
            {
                CommandText = SELECT_SQL + " where IndexNumber = @id;"
            };
            command.Parameters.AddWithValue("id", id);
            return getResults(command).FirstOrDefault();
        }

        public IEnumerable<Student> GetStudents()
        {
            var command = new SqlCommand
            {
                CommandText = SELECT_SQL + ";"
            };
            return getResults(command);
        }

        public void UpdateStudent(int id, Student student)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Student> getResults(SqlCommand command)
        {
            var list = new List<Student>();
            using (var connection = new SqlConnection(CONNECTION_STRING))
            using (command)
            {
                command.Connection = connection;
                connection.Open();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var student = addStudent(dataReader);
                    list.Add(student);
                }
            }
            return list;
        }

        private Student addStudent(SqlDataReader dataReader)
        {
            return new Student()
            {
                FirstName = dataReader["FirstName"].ToString(),
                LastName = dataReader["LastName"].ToString(),
                DateOfBirth = DateTime.Parse(dataReader["BirthDate"].ToString()),
                Faculty = dataReader["Name"].ToString(),
                Semester = Int32.Parse(dataReader["Semester"].ToString())
            };
        }
    }
}
