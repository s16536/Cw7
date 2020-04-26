using System.Net;
using lab05.DAL;
using lab05.DTOs;
using lab05.DTOs.Requests;
using lab05.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab05.Controllers
{
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        [Route("api/enrollments")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var student = new Student()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.BirthDate,
                Faculty = request.Studies,
                IndexNumber = request.IndexNumber

            };

            var enrollment = _dbService.AddStudent(student);
            if (enrollment == null)
            {
                return BadRequest();
            }

            return Ok(enrollment);
        }


        [HttpPost]
        [Route("api/enrollments/promotions")]
        public IActionResult PromoteStudents(PromoteStudentsRequest request)
        {
            var enrollment = _dbService.PromoteStudents(request);
            if (enrollment == null)
            {
                return NotFound();
            }

            return Created("api/enrollments/promotions", enrollment);
        }

    }
}