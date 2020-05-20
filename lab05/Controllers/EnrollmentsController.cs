using System.Net;
using lab05.DTOs;
using lab05.DTOs.Requests;
using lab05.Models;
using lab05.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab05.Controllers
{
    [ApiController]
    [Authorize(Roles = "employee")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService _studentsDbService;

        public EnrollmentsController(IStudentsDbService studentsDbService)
        {
            _studentsDbService = studentsDbService;
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

            var enrollment = _studentsDbService.AddStudent(student);
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
            var enrollment = _studentsDbService.PromoteStudents(request);
            if (enrollment == null)
            {
                return NotFound();
            }

            return Created("api/enrollments/promotions", enrollment);
        }

    }
}