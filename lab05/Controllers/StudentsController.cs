using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using lab05.DAL;
using lab05.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab05.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudent()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            var student = _dbService.GetStudent(id);
            if (student == null)
            {
                return NotFound("Nie znaleziono studenta"); ;
            }
            return Ok(student);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            _dbService.AddStudent(student);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            _dbService.UpdateStudent(id, student);
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            _dbService.DeleteStudent(id);
            return Ok("Usuwanie ukończone");
        }
    }
}