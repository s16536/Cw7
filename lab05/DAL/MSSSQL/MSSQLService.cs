﻿using lab05.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using lab05.DAL.MSSSQL;

namespace lab05.DAL
{
    public class MSSQLService : IDbService
    {
        private const string ConnectionString = "Data Source=db-mssql;Initial Catalog=s16536;Integrated Security=True";
        
        private const string SelectSql = "select FirstName, LastName, BirthDate, Semester, Name from Student s left join Enrollment e on s.IdEnrollment = e.IdEnrollment left join Studies studies on e.IdStudy = studies.IdStudy";

        public Enrollment AddStudent(Student student)
        {
            return new AddStudentService().AddStudent(ConnectionString, student);
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
    }
}