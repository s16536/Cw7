﻿using lab05.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab05.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string id);
        public void AddStudent(Student student);
        public void DeleteStudent(int id);
        public void UpdateStudent(int id, Student student);
    }
}