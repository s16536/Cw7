using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using lab05.DTOs.Requests;
using lab05.Models;
using lab05.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace lab05.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _studentsDbService;
        private readonly IConfiguration _configuration;

        public StudentsController(IStudentsDbService studentsDbService, IConfiguration configuration)
        {
            _studentsDbService = studentsDbService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetStudent()
        {
            return Ok(_studentsDbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            var student = _studentsDbService.GetStudent(id);
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
            _studentsDbService.AddStudent(student);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            _studentsDbService.UpdateStudent(id, student);
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            _studentsDbService.DeleteStudent(id);
            return Ok("Usuwanie ukończone");
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginRequestDto request)
        {

            var claims = _studentsDbService.Login(request);
            if (claims == null)
            {
                return Unauthorized();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience:"Students", 
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials:creds
            );
            return Ok(new
            {
                token= new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}