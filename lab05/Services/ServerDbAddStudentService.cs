using System;
using System.Data.SqlClient;
using lab05.Models;

namespace lab05.Services
{
    public class ServerDbAddStudentService
    {
        private Student _student;
        private SqlCommand _command;

        public Enrollment AddStudent(string connectionString, Student student)
        {
            _student = student;
            using (var connection = new SqlConnection(connectionString))
            using (_command = new SqlCommand())
            {
                _command.Connection = connection;
                connection.Open();
                var tran = connection.BeginTransaction();
                _command.Transaction = tran;

                if (!IsStudentIndexNumberUnique())
                {
                    tran.Rollback();
                    return null;
                }

                int? idStudies = GetStudiesId();
                if (idStudies == null)
                {
                    tran.Rollback();
                    return null;
                }

                Enrollment enrollment = GetEnrollment((int) idStudies);
                AddStudent(enrollment);
                tran.Commit();
                return enrollment;
            }
        }

        private bool IsStudentIndexNumberUnique()
        {
            var indexNumber = _student.IndexNumber;
            _command.CommandText = "select * from Student where indexNumber = @indexNumber;";
            _command.Parameters.AddWithValue("indexNumber", indexNumber);
            using (var dataReader = _command.ExecuteReader())
            {
                return !dataReader.Read();
            }
        }

        private int? GetStudiesId()
        {
            var name = _student.Faculty;
            _command.CommandText = "select IdStudy from Studies where name = @name;";
            _command.Parameters.AddWithValue("name", name);
            using (var dataReader = _command.ExecuteReader())
            {
                if (!dataReader.Read())
                {
                    return null;
                }

                return (int) dataReader["IdStudy"];
            }
        }

        private Enrollment GetEnrollment(int idStudy)
        {
            _command.CommandText = "select * from Enrollment where IdStudy = @idStudy and Semester = 1;";
            _command.Parameters.AddWithValue("idStudy", idStudy);
            using (var dataReader = _command.ExecuteReader())
            {
                if (dataReader.Read())
                {
                    var idEnrollment = (int) dataReader["IdEnrollment"];
                    _command.Parameters.AddWithValue("@idEnrollment", idEnrollment);
                    return new Enrollment()
                    {
                        IdEnrollment = idEnrollment,
                        Semester = (int) dataReader["Semester"],
                        IdStudy = (int) dataReader["IdStudy"],
                        StartDate = (DateTime) dataReader["StartDate"]
                    };
                }
            }
            return AddNewEnrollment(idStudy);
        }

        private Enrollment AddNewEnrollment(int idStudy)
        {
            int idEnrollment = 1;
            _command.CommandText = "select max(ISNULL(IdEnrollment,0)) from Enrollment;";
            using (var dataReader = _command.ExecuteReader())
            {
                if (dataReader.Read())
                {
                    idEnrollment = (int) dataReader[0] + 1;
                }
            }

            _command.CommandText = "insert into Enrollment values (@idEnrollment, 1, 1, @date);";
            var startDate = DateTime.Now;
            _command.Parameters.AddWithValue("date", startDate);
            _command.Parameters.AddWithValue("@idEnrollment", idEnrollment);
            _command.ExecuteNonQuery();
            return new Enrollment()
            {
                IdEnrollment = idEnrollment,
                Semester = 1,
                IdStudy = idStudy,
                StartDate = startDate,
            };
        }

        private void AddStudent(Enrollment enrollment)
        {
            _command.CommandText =
                "insert into Student values(@indexNumber, @firstName, @lastName, @birthDate, @idEnrollment);";
            _command.Parameters.AddWithValue("firstName", _student.FirstName);
            _command.Parameters.AddWithValue("lastName", _student.LastName);
            _command.Parameters.AddWithValue("birthDate", _student.DateOfBirth);
            _command.ExecuteNonQuery();
        }
    }
}