using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using lab05.DTOs.Requests;
using lab05.Models;
using Microsoft.AspNetCore.Authentication;

namespace lab05.Services
{
    public class ServerDbService : IStudentsDbService
    {
        private const string ConnectionString = "Data Source=db-mssql;Initial Catalog=s16536;Integrated Security=True";
        
        private const string SelectSql = "select FirstName, LastName, BirthDate, Semester, Name from Student s left join Enrollment e on s.IdEnrollment = e.IdEnrollment left join Studies studies on e.IdStudy = studies.IdStudy";

        public Enrollment AddStudent(Student student)
        {
            return new ServerDbAddStudentService().AddStudent(ConnectionString, student);
        }

        public void DeleteStudent(int id)
        {
            throw new NotImplementedException();
        }

        public Student GetStudent(string id)
        {
            var command = new SqlCommand
            {
                CommandText = SelectSql + " where IndexNumber = @id;"
            };
            command.Parameters.AddWithValue("id", id);
            return GetResults(command).FirstOrDefault();
        }

        public IEnumerable<Student> GetStudents()
        {
            var command = new SqlCommand
            {
                CommandText = SelectSql + ";"
            };
            return GetResults(command);
        }

        public void UpdateStudent(int id, Student student)
        {
            throw new NotImplementedException();
        }

        public Enrollment PromoteStudents(PromoteStudentsRequest request)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                command.CommandText = "exec PromoteStudents @studies, @semester;";
                command.Parameters.AddWithValue("studies", request.Studies);
                command.Parameters.AddWithValue("semester", request.Semester);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    return null;
                }

                command.CommandText = "select * from Enrollment where IdStudy = (select max(IdStudy) from Studies where Name = @studies) AND Semester = @Semester + 1 order by IdEnrollment";

                using (var dataReader = command.ExecuteReader())
                {
                    if (!dataReader.Read())
                    {
                        return null;
                    }
                    return new Enrollment()
                    {
                        IdEnrollment = (int) dataReader["IdEnrollment"],
                        IdStudy = (int) dataReader["IdStudy"],
                        Semester = (int) dataReader["Semester"],
                        StartDate = (DateTime) dataReader["StartDate"]
                    };

                }
            }
        }

        public AuthenticationResult Login(LoginRequestDto request)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                command.CommandText = "select Role from Student where IndexNumber = @index and Password = @password;";
                command.Parameters.AddWithValue("index", request.Login);
                command.Parameters.AddWithValue("password", request.Password);
                return Authenticate(command);
            };
        }

        public AuthenticationResult Login(string token)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                command.CommandText = "select IndexNumber, Role from Student s " +
                                      "left join RefreshToken r on  r.StudentID = s.IndexNumber " +
                                      "where Token=@token and ValidTo > GETDATE();";
                command.Parameters.AddWithValue("token", token);

                return Authenticate(command);
            }
        }

        private AuthenticationResult Authenticate(SqlCommand command)
        {
            var result = new AuthenticationResult();
            using (var dataReader = command.ExecuteReader())
            {
                if (!dataReader.Read())
                {
                    return null;
                }

                if (!command.Parameters.Contains("index"))
                {
                    command.Parameters.AddWithValue("index", dataReader["IndexNumber"].ToString());
                }

                result.Claims = new[]
                {
                    new Claim(ClaimTypes.Name, command.Parameters["index"].Value.ToString()),
                    new Claim(ClaimTypes.Role, dataReader["Role"].ToString())
                };
            }

            result.RefreshToken = Guid.NewGuid().ToString();
            AddToken(command, result.RefreshToken);
            return result;
        }

        private IEnumerable<Student> GetResults(SqlCommand command)
        {
            var list = new List<Student>();
            using (var connection = new SqlConnection(ConnectionString))
            using (command)
            {
                command.Connection = connection;
                connection.Open();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var student = CreateStudent(dataReader);
                    list.Add(student);
                }
            }
            return list;
        }

        private Student CreateStudent(SqlDataReader dataReader)
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

        private void AddToken(SqlCommand command, string token)
        {
            command.CommandText =
                "insert into RefreshToken(Token, StudentId, ValidTo) values (@newToken, @index, @validTo);";
            command.Parameters.AddWithValue("newToken", token);
            command.Parameters.AddWithValue("validTo", DateTime.Now.AddDays(1));
            command.ExecuteNonQuery();
        }
    }
}
